using Compras.Models;

namespace Compras.Repositories
{
    public interface ITiendaRepository
    {
        Task<LoginResponse> Login(UserLogin user);

        Task<RegisterResponse> Register(Usuario usuario);

        Task<List<Articulo>> GetArticulosAsync();

        Task<bool> AddArticuloToCarritoAsync(int idUsuario, int idArticulo, decimal price, int cantidad);

        Task<CarritoResponse> GetCarritoByUsuarioId(int idUsuario);

        Task<OrdenResponse> RegistrarOrden(int idUsuario, List<ArticuloOrden> articulos);

        Task<List<OrdenDetalleResponse>> GetOrdenesByUsuarioId(int idUsuario);
    }
}
