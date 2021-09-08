using DevStore.Catalog.API.Models;
using DevStore.Core.DomainObjects;
using DevStore.Core.Messages.Integration;
using DevStore.MessageBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevStore.Catalog.API.Services
{
    public class CatalogIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;

        public CatalogIntegrationHandler(IServiceProvider serviceProvider, IMessageBus bus)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            SetSubscribers();
            return Task.CompletedTask;
        }

        private void SetSubscribers()
        {
            _bus.SubscribeAsync<OrderAuthorizedIntegrationEvent>("OrderAuthorized", async request =>
                await WriteDownInventory(request));
        }

        private async Task WriteDownInventory(OrderAuthorizedIntegrationEvent message)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var productsWithAvailableStock = new List<Product>();
                var productRepository = scope.ServiceProvider.GetRequiredService<IProductRepository>();

                var productsId = string.Join(",", message.Items.Select(c => c.Key));
                var products = await productRepository.GetProductsById(productsId);

                if (products.Count != message.Items.Count)
                {
                    CancelOrderWithoutStock(message);
                    return;
                }

                foreach (var produto in products)
                {
                    var quantidadeProduto = message.Items.FirstOrDefault(p => p.Key == produto.Id).Value;

                    if (produto.IsAvailable(quantidadeProduto))
                    {
                        produto.TakeFromInventory(quantidadeProduto);
                        productsWithAvailableStock.Add(produto);
                    }
                }

                if (productsWithAvailableStock.Count != message.Items.Count)
                {
                    CancelOrderWithoutStock(message);
                    return;
                }

                foreach (var produto in productsWithAvailableStock)
                {
                    productRepository.Update(produto);
                }

                if (!await productRepository.UnitOfWork.Commit())
                {
                    throw new DomainException($"Problemas ao atualizar estoque do pedido {message.OrderId}");
                }

                var pedidoBaixado = new OrderLoweredStockIntegrationEvent(message.CustomerId, message.OrderId);
                await _bus.PublishAsync(pedidoBaixado);
            }
        }

        public async void CancelOrderWithoutStock(OrderAuthorizedIntegrationEvent message)
        {
            var orderCancelled = new OrderCanceledIntegrationEvent(message.CustomerId, message.OrderId);
            await _bus.PublishAsync(orderCancelled);
        }
    }
}