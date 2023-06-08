using LanchesMac.Context;

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
    }
}
