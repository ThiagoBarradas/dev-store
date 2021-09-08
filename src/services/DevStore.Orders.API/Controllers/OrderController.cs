using DevStore.Core.Mediator;
using DevStore.Orders.API.Application.Commands;
using DevStore.Orders.API.Application.Queries;
using DevStore.WebAPI.Core.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using DevStore.WebAPI.Core.User;

namespace DevStore.Orders.API.Controllers
{
    [Authorize, Route("orders")]
    public class OrderController : MainController
    {
        private readonly IMediatorHandler _mediator;
        private readonly IAspNetUser _user;
        private readonly IOrderQueries _orderQueries;

        public OrderController(IMediatorHandler mediator,
            IAspNetUser user,
            IOrderQueries orderQueries)
        {
            _mediator = mediator;
            _user = user;
            _orderQueries = orderQueries;
        }

        [HttpPost("")]
        public async Task<IActionResult> AddOrder(AddOrderCommand pedido)
        {
            pedido.CustomerId = _user.GetUserId();
            return CustomResponse(await _mediator.SendCommand(pedido));
        }

        [HttpGet("last")]
        public async Task<IActionResult> LastOrder()
        {
            var pedido = await _orderQueries.GetLastOrder(_user.GetUserId());

            return pedido == null ? NotFound() : CustomResponse(pedido);
        }

        [HttpGet("customers")]
        public async Task<IActionResult> Customers()
        {
            var pedidos = await _orderQueries.GetByCustomerId(_user.GetUserId());

            return pedidos == null ? NotFound() : CustomResponse(pedidos);
        }
    }
}