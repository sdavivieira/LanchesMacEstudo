using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class PedidoController : Controller
    {
       private readonly  IPedidoRepository _pedidoRepository;
       private readonly  CarrinhoCompra _carrinhocompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhocompra)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhocompra = carrinhocompra;
        }

        [Authorize]
        [HttpGet]
        public IActionResult Checkout() 
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            int totalItensPedido = 0;
            decimal precoTotalPedido = 0.0m;


            List<CarrinhoCompraItem> itens = _carrinhocompra.GetCarrinhoCompraItens();
            _carrinhocompra.CarrinhoCompraItens = itens;

            if(_carrinhocompra.CarrinhoCompraItens.Count == 0)
            {
                ModelState.AddModelError(" ", "Seu carrinho está vazio, que tal incluir um lanche?");
            }

            foreach(var item in itens)
            {
                totalItensPedido += item.Quantidade;
                precoTotalPedido += (item.Lanche.Preco * item.Quantidade);
            }

            pedido.TotalItensPedido = totalItensPedido;
            pedido.PedidoTotal = precoTotalPedido;

            if (ModelState.IsValid)
            {
                _pedidoRepository.CriarPedido(pedido);

                ViewBag.CheckoutCompletoMensagem = "Obrigado pelo seu pedido  :)";
                ViewBag.TotalPedido = _carrinhocompra.GetCarrinhoCompraTotal();

                _carrinhocompra.LimparCarrinho();

                return View("~/Views/Pedido/CheckoutCompleto.cshtml", pedido);
            }
            return View(pedido);
        }
    }
}
