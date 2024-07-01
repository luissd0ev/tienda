using Compras.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Compras.Controllers.MarsystemsDemo;

namespace Compras.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpPost("orden")]
        public async Task<IActionResult> RegistrarOrden([FromBody] RegistrarOrdenRequest request)
        {
            try
            {
                var response = await _orderRepository.Add(request.IdUsuario, request.Articulos);
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
                var ordenes = await _orderRepository.Get(idUsuario);
                return Ok(ordenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
