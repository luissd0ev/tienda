using Compras.Models;
using Compras.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compras.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private IArticleRepository _articleRepository;
        public ArticleController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [HttpGet("Articles")]
        public async Task<ActionResult<List<Articulo>>> GetArticulos()
        {
            try
            {
                var articulos = await this._articleRepository.GetAll();
                return Ok(articulos);
            }
            catch (Exception ex)
            {
                // Aquí puedes manejar la excepción según tus necesidades
                // Por ejemplo, puedes registrar el error, devolver un mensaje de error específico, etc.
                return StatusCode(500, "Error al obtener los artículos: " + ex.Message);
            }
        }


    }
}
