using Compras.Models;

namespace Compras.Repositories
{
    public interface IArticleRepository
    {
        Task<List<Articulo>> GetAll();

        
    }
}
