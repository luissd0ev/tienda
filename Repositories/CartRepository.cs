using Compras.Helpers;
using Compras.Models;
using Microsoft.EntityFrameworkCore;

namespace Compras.Repositories
{
    public class CartRepository:ICart
    {

        private readonly MarsystemsDemoDbContext _context;
        private readonly Funcionalidades _funcionalidades;
        public CartRepository(MarsystemsDemoDbContext context, Funcionalidades funcionalidades)
        {
            _context = context;
            _funcionalidades = funcionalidades;
        }
        public async Task<Carritoscompra> Get(int idUsuario)
        {
            return await _context.Carritoscompras
                .FirstOrDefaultAsync(c => c.Idcarrito == idUsuario);
        }

        public async Task<bool> AddArticleToCart(int idUsuario, int idArticulo, decimal price, int cantidad)
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
    }
}
