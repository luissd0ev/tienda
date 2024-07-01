using Compras.Helpers;
using Compras.Models;
using Microsoft.EntityFrameworkCore;

namespace Compras.Repositories
{
    public class ArticleRepository:IArticleRepository
    {
        private readonly MarsystemsDemoDbContext _context;
        private readonly Funcionalidades _funcionalidades;
        public ArticleRepository(MarsystemsDemoDbContext context, Funcionalidades funcionalidades)
        {
            _context = context;
            _funcionalidades = funcionalidades;
        }

        public async Task<List<Articulo>> GetAll()
        {
            try
            {
                return await _context.Articulos.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener los artículos.", ex);
            }
        }
    
    }
}
