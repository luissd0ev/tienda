using Compras.Models;
using Compras.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compras.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private IUserRepository _userRepository;
        public UserController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLogin usuario)
        {
            try
            {
                LoginResponse response = await _userRepository.Get(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, devolver un mensaje de error específico, etc.
                return StatusCode(500, "Error al intentar iniciar sesión: " + ex.Message);
            }
        }

        // Método para registrar un nuevo usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] Usuario usuario)
        {
            try
            {
                RegisterResponse response = await _userRepository.Add(usuario);
                return Ok(response);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, devolver un mensaje de error específico, etc.
                return StatusCode(500, "Error al intentar registrar el usuario: " + ex.Message);
            }
        }

    }
}
