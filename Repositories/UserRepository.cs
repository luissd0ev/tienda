using Compras.Helpers;
using Compras.Models;
using Microsoft.EntityFrameworkCore;

namespace Compras.Repositories
{
    public class UserRepository:IUserRepository
    {


        private readonly MarsystemsDemoDbContext _context;
        private readonly Funcionalidades _funcionalidades;
        public UserRepository(MarsystemsDemoDbContext context, Funcionalidades funcionalidades)
        {
            _context = context;
            _funcionalidades = funcionalidades;
        }
        public async Task<LoginResponse> Get(UserLogin usuario)
        {
            var user = await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == usuario.Email);
            if (user == null)
            {
                return new LoginResponse { Message = "Credenciales inválidas.", IsSuccessful = false };
            }

            // Verificar la contraseña utilizando el helper
            if (!_funcionalidades.VerifyPassword(usuario.Passworduser, user.Passworduser))
            {
                return new LoginResponse { Message = "Credenciales inválidas.", IsSuccessful = false };
            }

            // Si el inicio de sesión es exitoso, construir la respuesta completa
            return new LoginResponse
            {
                Message = "Login exitoso.",
                IsSuccessful = true,
                UserId = user.Iduser,
                UserName = user.Nameuser

            };
        }
         
        public async Task<RegisterResponse> Add(Usuario usuario)
        {
            // Verificar si el email ya está registrado
            if (await _context.Usuarios.AnyAsync(u => u.Email == usuario.Email))
            {
                return new RegisterResponse { Message = "El email ya está registrado.", IsSuccessful = false };
            }

            // Encriptar la contraseña
            usuario.Passworduser = _funcionalidades.HashPassword(usuario.Passworduser);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Si el registro es exitoso, construir la respuesta completa
            return new RegisterResponse
            {
                Message = "Registro exitoso.",
                IsSuccessful = true
                // Puedes agregar más datos relevantes aquí
            };
        }

    }
}
