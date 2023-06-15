using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LanchesMac.Controllers
{
    public class PedidoController : Controller
    {
        private readonly IPedidoRepository _pedidoRepository;
        private readonly CarrinhoCompra _carrinhoCompra;

        public PedidoController(IPedidoRepository pedidoRepository, CarrinhoCompra carrinhoCompra)
        {
            _pedidoRepository = pedidoRepository;
            _carrinhoCompra = carrinhoCompra;
        }


        // Formulário de confirmação para o cliente
        public IActionResult Checkout()
        {
            return View();
        }


        // Trata o formulário de confirmação
        [HttpPost]
        public IActionResult Checkout(Pedido pedido)
        {
            return View();
        }
    }
}
