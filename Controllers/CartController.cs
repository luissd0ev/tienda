using Compras.Helpers;
using Compras.Models;
using Compras.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Compras.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private ICart _cartRepository;
        
        public CartController(ICart cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpPost("AddToCarrito")]
        public async Task<IActionResult> AddToCarrito(int idUsuario, int idArticulo, decimal price, int cantidad)
        {
            try
            {
                bool added = await _cartRepository.AddArticleToCart(idUsuario, idArticulo, price, cantidad);
                if (added)
                {
                    return Ok(new { message = "Artículo agregado al carrito exitosamente." });
                }
                else
                {
                    return BadRequest(new { message = "El artículo ya está en el carrito." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error al intentar agregar artículo al carrito.", error = ex.Message });
            }
        }

        [HttpGet("carrito/{idUsuario}")]
        public async Task<IActionResult> GetCarrito(int idUsuario)
        {
            try
            {
                var response = await _cartRepository.Get(idUsuario);
                if (!response.IsSuccessful)
                {
                    return NotFound(response.Message);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("carrito/{idUsuario}/{idArticulo}")]
        public async Task<IActionResult> RemoveArticuloFromCarrito(int idUsuario, int idArticulo)
        {
            try
            {
                bool result = await _cartRepository.Remove(idUsuario, idArticulo);
                if (result)
                {
                    return Ok(new { Message = "Artículo eliminado del carrito con éxito." });
                }
                else
                {
                    return BadRequest(new { Message = "No se pudo eliminar el artículo del carrito." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
