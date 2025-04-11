using FinTrack.Domain.Contratos;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Interfaces.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/FinTrack/[controller]")]
    public class CategoriasController(ICategoriaRepositorio categoriaRepositorio, ILogger<CategoriasController> logger) : ControllerBase
    {
        private readonly ICategoriaRepositorio _categoriaRepositorio = categoriaRepositorio;
        private readonly ILogger<CategoriasController> _logger = logger;

        [HttpGet("obterTodas")]
        public async Task<ActionResult<IEnumerable<CategoriaContrato>>> ObterTodas()
        {
            try
            {
                var categorias = await _categoriaRepositorio.ObterTodasAsync();
                var categoriasContrato = categorias.Select(MapearParaContrato);
                return Ok(categoriasContrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar todas as categorias");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("obterPorId/{id}")]
        public async Task<ActionResult<Transacao>> ObterPorId(int id)
        {
            try
            {
                var categoria = await _categoriaRepositorio.ObterPorIdAsync(id);
                if (categoria == null) return NotFound($"Categoria com o id {id} não encontrada");

                return Ok(MapearParaContrato(categoria));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar categoria com id: {id}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost("salvarCategoria")]
        public async Task<IActionResult> CriarCategoria([FromBody] CriarCategoriaContrato categoriaContrato)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var transacao = new Categoria
                {
                    Descricao = categoriaContrato.Descricao,
                    Nome = categoriaContrato.Nome,
                };

                await _categoriaRepositorio.AdicionarAsync(transacao);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar categoria com payload: Nome= {Nome}; Descricao={Descricao}", categoriaContrato.Nome, categoriaContrato.Descricao);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("removerCategoria/{id}")]
        public async Task<IActionResult> RemoverCategoria(int id)
        {
            try
            {
                var categoria = await _categoriaRepositorio.ObterPorIdAsync(id);
                if (categoria == null) return NotFound($"Categoria com id {id} não encontrada");

                await _categoriaRepositorio.DeletarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao remover categoria com id: {id}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("editarCategoria/{id}")]
        public async Task<IActionResult> EditarCategoria(int id, [FromQuery] string? nome, [FromQuery] string? descricao)
        {
            try
            {
                var categoria = await _categoriaRepositorio.ObterPorIdAsync(id);
                if (categoria == null) return NotFound($"Categoria com id: {id} não encontrada");
                categoria.Nome = nome ?? categoria.Nome;
                categoria.Descricao = descricao ?? categoria.Descricao;
                await _categoriaRepositorio.AtualizarAsync(categoria);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Não foi possível atualizar a categoria com id: {id}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        private static CategoriaContrato MapearParaContrato(Categoria categoria)
        {
            return new CategoriaContrato
            {
                Id = categoria.Id,
                Nome = categoria.Nome,
                Descricao = categoria.Descricao,
            };
        }
    }
}
