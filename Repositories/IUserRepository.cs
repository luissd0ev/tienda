using Compras.Models;

namespace Compras.Repositories
{
    public interface IUserRepository
    {
        Task<LoginResponse> Get(UserLogin user);
        Task<RegisterResponse> Add(Usuario usuario);
 
    }
}
