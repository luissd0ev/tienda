using Compras.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using Compras.Repositories;

namespace Compras.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarsystemsDemo : ControllerBase
    {

        public class RegistrarOrdenRequest
        {
            public int IdUsuario { get; set; }
            public List<ArticuloOrden> Articulos { get; set; } = new List<ArticuloOrden>();
        }

        private ITiendaRepository _tiendaRepository;
        public MarsystemsDemo( ITiendaRepository tiendaRepository)
        {
            _tiendaRepository = tiendaRepository;
        }

        // Método para registrar un nuevo usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            try
            {
                RegisterResponse response = await _tiendaRepository.Register(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, devolver un mensaje de error específico, etc.
                return StatusCode(500, "Error al intentar registrar el usuario: " + ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin usuario)
        {
            try
            {
                LoginResponse response = await _tiendaRepository.Login(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, devolver un mensaje de error específico, etc.
                return StatusCode(500, "Error al intentar iniciar sesión: " + ex.Message);
            }
        }


        [HttpGet("Articulos")]
        public async Task<ActionResult<List<Articulo>>> GetArticulos()
        {
            try
            {
                var articulos = await this._tiendaRepository.GetArticulosAsync();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, devolver un mensaje de error específico, etc.
                return StatusCode(500, "Error al obtener los artículos: " + ex.Message);
            }
        }

        [HttpPost("AddToCarrito")]
        public async Task<IActionResult> AddToCarrito(int idUsuario, int idArticulo, decimal price, int cantidad)
        {
            try
            {
                bool added = await _tiendaRepository.AddArticuloToCarritoAsync(idUsuario, idArticulo, price, cantidad);
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
                var response = await _tiendaRepository.GetCarritoByUsuarioId(idUsuario);
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

        [HttpPost("orden")]
        public async Task<IActionResult> RegistrarOrden([FromBody] RegistrarOrdenRequest request)
        {
            try
            {
                var response = await _tiendaRepository.RegistrarOrden(request.IdUsuario, request.Articulos);
                if (!response.IsSuccessful)
                {
                    return BadRequest(response.Message);
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("ordenes/{idUsuario}")]
        public async Task<IActionResult> GetOrdenes(int idUsuario)
        {
            try
            {
                var ordenes = await _tiendaRepository.GetOrdenesByUsuarioId(idUsuario);
                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    }
}
