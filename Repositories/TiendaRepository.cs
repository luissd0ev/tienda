using Compras.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Compras.Helpers;

namespace Compras.Repositories
{
    public class UserLogin
    {
        public string Email { get; set; }
        public string Passworduser { get; set; }
    }

    public class RegisterResponse
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        // Puedes agregar más propiedades según sea necesario
    }
    public class LoginResponse
    {
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
        public int UserId { get; set; }  // ID del usuario (si es necesario)
        public string UserName { get; set; }  // Nombre del usuario
                                              // Puedes agregar más propiedades según sea necesario
    }
    public class TiendaRepository:ITiendaRepository
    {

        private readonly MarsystemsDemoDbContext _context;
        private readonly Funcionalidades _funcionalidades;
        public TiendaRepository(MarsystemsDemoDbContext context, Funcionalidades funcionalidades)
        {
            _context = context;
            _funcionalidades = funcionalidades;
        }
        public async Task<LoginResponse> Login(UserLogin usuario)
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

        public async Task<RegisterResponse> Register(Usuario usuario)
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

        public async Task<List<Articulo>> GetArticulosAsync()
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

        public async Task<Carritoscompra> GetCarritoAsync(int idUsuario)
        {
            return await _context.Carritoscompras
                .FirstOrDefaultAsync(c => c.Idcarrito == idUsuario);
        }

        public async Task<bool> AddArticuloToCarritoAsync(int idUsuario, int idArticulo, decimal price)
        {
            try
            {
                // Verificar si el artículo existe en la tabla Articulos
                var articulo = await _context.Articulos.FindAsync(idArticulo);
                if (articulo == null)
                {
                    throw new Exception("El artículo no existe.");
                }

                // Verificar si ya existe un registro del artículo en el carrito del usuario
                var existingItem = await _context.Carritoscompras
                    .FirstOrDefaultAsync(c => c.Idcarrito == idUsuario && c.Idarticulo == idArticulo);

                if (existingItem != null)
                {
                    // Si el artículo ya está en el carrito, puedes actualizar la cantidad o el precio si es necesario
                    // Pero en este caso, parece que solo mantenemos un registro por artículo en el carrito
                    return false; // Si ya está en el carrito, no hacemos nada (puedes ajustar esto según tu lógica)
                }
                else
                {
                    // Si el artículo no está en el carrito, agregarlo como un nuevo item
                    var nuevoArticuloEnCarrito = new Carritoscompra
                    {
                        Idcarrito = idUsuario,
                        Idarticulo = idArticulo,
                        Price = price
                    };
                    _context.Carritoscompras.Add(nuevoArticuloEnCarrito);
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, lanzar una excepción personalizada, etc.
                throw new Exception("Error al agregar artículo al carrito.", ex);
            }
        }




    }
}
