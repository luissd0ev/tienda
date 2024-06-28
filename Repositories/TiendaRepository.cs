using Compras.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Compras.Helpers;

namespace Compras.Repositories
{

    public class OrdenResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int IdOrder { get; set; }
    }

    public class OrdenDetalleResponse
    {
        public int IdOrder { get; set; }
        public DateTime Fecha { get; set; }
        public decimal Total { get; set; }
        public List<ArticuloOrdenResponse> Articulos { get; set; } = new List<ArticuloOrdenResponse>();
    }

    public class ArticuloOrdenResponse
    {
        public int IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }

    public class ArticuloOrden
    {
        public int IdArticulo { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
    public class CarritoResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public List<CarritoItemResponse> Items { get; set; } = new List<CarritoItemResponse>();
    }

    public class CarritoItemResponse
    {
        public int IdArticulo { get; set; }
        public string NombreArticulo { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }
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

        public async Task<bool> AddArticuloToCarritoAsync(int idUsuario, int idArticulo, decimal price, int cantidad)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Verificar si el artículo existe en la tabla Articulos
                var articulo = await _context.Articulos.FindAsync(idArticulo);
                if (articulo == null)
                {
                    throw new Exception("El artículo no existe.");
                }

                // Verificar si hay suficientes unidades disponibles en el inventario
                if (articulo.Quantityart < cantidad)
                {
                    throw new Exception($"Stock insuficiente para el artículo {articulo.Nameart}. Cantidad disponible: {articulo.Quantityart}");
                }

                // Verificar si ya existe un registro del artículo en el carrito del usuario
                var existingItem = await _context.Carritoscompras
                    .FirstOrDefaultAsync(c => c.Idcarrito == idUsuario && c.Idarticulo == idArticulo);

                if (existingItem != null)
                {
                    // Si el artículo ya está en el carrito, actualizar la cantidad
                    existingItem.Cantidad += cantidad;
                    _context.Carritoscompras.Update(existingItem);
                }
                else
                {
                    // Si el artículo no está en el carrito, agregarlo como un nuevo item
                    var nuevoArticuloEnCarrito = new Carritoscompra
                    {
                        Idcarrito = idUsuario,
                        Idarticulo = idArticulo,
                        Price = price,
                        Cantidad = cantidad
                    };
                    _context.Carritoscompras.Add(nuevoArticuloEnCarrito);
                }

                // Actualizar el inventario del artículo
                articulo.Quantityart -= cantidad;
                _context.Articulos.Update(articulo);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                // Manejar la excepción según tus necesidades
                // Puedes registrar el error, lanzar una excepción personalizada, etc.
                await transaction.RollbackAsync();
                throw new Exception("Error al agregar artículo al carrito.", ex);
            }
        }
        public async Task<CarritoResponse> GetCarritoByUsuarioId(int idUsuario)
        {
            try
            {
                // Verificar si el usuario existe
                var usuario = await _context.Usuarios.FindAsync(idUsuario);
                if (usuario == null)
                {
                    return new CarritoResponse
                    {
                        IsSuccessful = false,
                        Message = "Usuario no encontrado."
                    };
                }

                // Consultar los artículos en el carrito del usuario
                var carritoItems = await _context.Carritoscompras
                                                 .Where(c => c.Idcarrito == idUsuario)
                                                 .Select(c => new CarritoItemResponse
                                                 {
                                                     IdArticulo = c.Idarticulo,
                                                     NombreArticulo = c.IdarticuloNavigation.Nameart,
                                                     Precio = c.Price,
                                                     Cantidad = c.Cantidad
                                                 })
                                                 .ToListAsync();

                return new CarritoResponse
                {
                    IsSuccessful = true,
                    Items = carritoItems
                };
            }
            catch (Exception ex)
            {
                return new CarritoResponse
                {
                    IsSuccessful = false,
                    Message = $"Error al consultar el carrito: {ex.Message}"
                };
            }
        }

        public async Task<OrdenResponse> RegistrarOrden(int idUsuario, List<ArticuloOrden> articulos)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Crear la nueva orden
                var nuevaOrden = new Ordene
                {
                    Iduser = idUsuario,
                    Fecha = DateTime.Now,
                    Total = articulos.Sum(a => a.Precio * a.Cantidad)
                };
                _context.Ordenes.Add(nuevaOrden);
                await _context.SaveChangesAsync();

                // Crear las entradas en Ordenes_Articulos y actualizar inventario
                foreach (var articulo in articulos)
                {
                    var articuloDb = await _context.Articulos.FindAsync(articulo.IdArticulo);
                    if (articuloDb == null || articuloDb.Quantityart < articulo.Cantidad)
                    {
                        await transaction.RollbackAsync();
                        return new OrdenResponse
                        {
                            IsSuccessful = false,
                            Message = $"Stock insuficiente para el artículo {articuloDb?.Nameart ?? "desconocido"}."
                        };
                    }

                    // Actualizar la cantidad del artículo en el inventario
                    articuloDb.Quantityart -= articulo.Cantidad;
                    _context.Articulos.Update(articuloDb);

                    // Añadir entrada en Ordenes_Articulos
                    _context.OrdenesArticulos.Add(new OrdenesArticulo
                    {
                        Idorder = nuevaOrden.Idorder,
                        Idarticulo = articulo.IdArticulo,
                        Cantidad = articulo.Cantidad
                    });
                }
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new OrdenResponse
                {
                    IsSuccessful = true,
                    Message = "Orden registrada con éxito.",
                    IdOrder = nuevaOrden.Idorder
                };
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new OrdenResponse
                {
                    IsSuccessful = false,
                    Message = $"Error al registrar la orden: {ex.Message}"
                };
            }
        }

        public async Task<List<OrdenDetalleResponse>> GetOrdenesByUsuarioId(int idUsuario)
        {
            try
            {
                var ordenes = await _context.Ordenes
                                            .Where(o => o.Iduser == idUsuario)
                                            .Select(o => new OrdenDetalleResponse
                                            {
                                                IdOrder = o.Idorder,
                                                Fecha = o.Fecha,
                                                Total = o.Total,
                                                Articulos = o.OrdenesArticulos.Select(oa => new ArticuloOrdenResponse
                                                {
                                                    IdArticulo = oa.Idarticulo,
                                                    NombreArticulo = oa.IdarticuloNavigation.Nameart,
                                                    Precio = oa.IdarticuloNavigation.Priceart,
                                                    Cantidad = oa.Cantidad
                                                }).ToList()
                                            })
                                            .ToListAsync();

                return ordenes;
            }
            catch (Exception ex)
            {
                // Manejar la excepción de acuerdo a tus necesidades
                return new List<OrdenDetalleResponse>();
            }
        }
    }
}
