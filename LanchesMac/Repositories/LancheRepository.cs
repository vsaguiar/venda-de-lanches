using LanchesMac.Context;
using LanchesMac.Models;
using LanchesMac.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LanchesMac.Repositories
{
    public class LancheRepository : ILancheRepository
    {
        private readonly AppDbContext _context; // somente leitura e visível apenas dentro dessa classe
        public LancheRepository(AppDbContext contexto)
        {
            _context = contexto;
        }

        // Incluindo a categoria no lanche obtido
        public IEnumerable<Lanche> Lanches => _context.Lanches.Include(c => c.Categoria);

        // Incluindo a categoria no lanche preferido
        public IEnumerable<Lanche> LanchesPreferidos => _context.Lanches.
            Where(x => x.IsLanchePreferido).Include(c => c.Categoria);

        public Lanche GetLancheById(int lancheId)
        {
            return _context.Lanches.FirstOrDefault(x => x.LancheId == lancheId);
        }
    }
}
