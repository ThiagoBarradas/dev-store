using System.Threading.Tasks;
using DevStore.Core.Mediator;
using DevStore.Orders.API.Application.Commands;
using DevStore.Orders.API.Application.Queries;
using DevStore.WebAPI.Core.Controllers;
using DevStore.WebAPI.Core.Usuario;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevStore.Orders.API.Controllers
{
    [Authorize]
    public class PedidoController : MainController
    {
        private readonly IMediatorHandler _mediator;
        private readonly IAspNetUser _user;
        private readonly IPedidoQueries _pedidoQueries;

        public PedidoController(IMediatorHandler mediator,
            IAspNetUser user,
            IPedidoQueries pedidoQueries)
        {
            _mediator = mediator;
            _user = user;
            _pedidoQueries = pedidoQueries;
        }

        [HttpPost("order")]
        public async Task<IActionResult> AdicionarPedido(AddOrderCommand pedido)
        {
            pedido.ClientId = _user.GetUserId();
            return CustomResponse(await _mediator.SendCommand(pedido));
        }

        [HttpGet("order/ultimo")]
        public async Task<IActionResult> UltimoPedido()
        {
            var pedido = await _pedidoQueries.ObterUltimoPedido(_user.GetUserId());

            return pedido == null ? NotFound() : CustomResponse(pedido);
        }

        [HttpGet("order/lista-cliente")]
        public async Task<IActionResult> ListaPorCliente()
        {
            var pedidos = await _pedidoQueries.ObterListaPorClienteId(_user.GetUserId());

            return pedidos == null ? NotFound() : CustomResponse(pedidos);
        }
    }
}