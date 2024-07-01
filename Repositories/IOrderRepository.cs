using Compras.Models;

namespace Compras.Repositories
{
    public interface IOrderRepository
    {
        public Task<OrdenResponse> Add(int idUsuario, List<ArticuloOrden> articulos);
        public Task<List<OrdenDetalleResponse>> Get(int idUsuario);
    }
}
