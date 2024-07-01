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

    }
}
