using LanchesMac.Context;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Models
{
    public class CarrinhoCompra
    {
        private readonly AppDbContext _context;

        public CarrinhoCompra(AppDbContext context)
        {
            _context = context;
        }

        public string CarrinhoCompraId { get; set; }
        public List<CarrinhoCompraItem> CarrinhoCompraItems { get; set; }

        public static CarrinhoCompra GetCarrinho(IServiceProvider services)
        {
            // Define uma sessão
            ISession session = services.GetRequiredService<HttpContextAccessor>()?.HttpContext.Session;

            // Obtém um serviço do tipo do nosso contexto
            var context = services.GetService<AppDbContext>();

            // Obtém ou gera o ID do carrinho
            string carrinhoId = session.GetString("CarrinhoId") ?? Guid.NewGuid().ToString();

            // Atribui o ID do carrinho na sessão
            session.SetString("CarrinhoId", carrinhoId);

            // Retorna o carrinho com o contexto e o ID atribuído ou obtido
            return new CarrinhoCompra(context)
            {
                CarrinhoCompraId = carrinhoId,
            };
        }

        public void AdicionarAoCarrinho(Lanche lanche)
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItens.SingleOrDefault(
                s => s.Lanche.LancheId == lanche.LancheId &&
                s.CarrinhoCompraId == CarrinhoCompraId);

            // Caso o lanche não exista no carrinho
            if (carrinhoCompraItem == null)
            {
                carrinhoCompraItem = new CarrinhoCompraItem
                {
                    CarrinhoCompraId = CarrinhoCompraId, // id do carrinho = id da session
                    Lanche = lanche,
                    Quantidade = 1
                };
                _context.CarrinhoCompraItens.Add(carrinhoCompraItem);
            }
            else
            {
                carrinhoCompraItem.Quantidade++;
            }
            _context.SaveChanges();
        }

        public void RemoverDoCarrinho(Lanche lanche)
        {
            var carrinhoCompraItem = _context.CarrinhoCompraItens.SingleOrDefault(
                s => s.Lanche.LancheId == lanche.LancheId &&
                s.CarrinhoCompraId == CarrinhoCompraId);

            if (carrinhoCompraItem != null)
            {
                if (carrinhoCompraItem.Quantidade > 1)
                {
                    carrinhoCompraItem.Quantidade--;
                }
                else
                {
                    _context.CarrinhoCompraItens.Remove(carrinhoCompraItem);
                }
            }
            _context.SaveChanges();
        }

        // Obtendo todos os itens do carrinho
        public List<CarrinhoCompraItem> GetCarrinhoCompraItens()
        {
            if (CarrinhoCompraItems != null)
            {
                return CarrinhoCompraItems;
            }
            else
            {
                CarrinhoCompraItems = _context.CarrinhoCompraItens
                                    .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                                    .Include(s => s.Lanche)
                                    .ToList();
                return CarrinhoCompraItems;
            }
        }

        public void LimparCarrinho()
        {
            var carrinhoItens = _context.CarrinhoCompraItens.Where(carrinho => carrinho.CarrinhoCompraId == CarrinhoCompraId);

            _context.CarrinhoCompraItens.RemoveRange(carrinhoItens);
            _context.SaveChanges();
        }

        // Obtendo o valor total do carrinho
        public decimal GetCarrinhoCompraTotal()
        {
            var total = _context.CarrinhoCompraItens
                        .Where(c => c.CarrinhoCompraId == CarrinhoCompraId)
                        .Select(c => c.Lanche.Preco * c.Quantidade).Sum();

            return total;
        }
    }
}
