using Compras.Helpers;
using Compras.Models;
using Microsoft.EntityFrameworkCore;

namespace Compras.Repositories
{
    public class OrderRepository
    {
        private readonly MarsystemsDemoDbContext _context;
        private readonly Funcionalidades _funcionalidades;
        public OrderRepository(MarsystemsDemoDbContext context, Funcionalidades funcionalidades)
        {
            _context = context;
            _funcionalidades = funcionalidades;
        }

        public async Task<OrdenResponse> Add(int idUsuario, List<ArticuloOrden> articulos)
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

        public async Task<List<OrdenDetalleResponse>> Get(int idUsuario)
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
