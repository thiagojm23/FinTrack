using FinTrack.Domain.Contratos;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Domain.Interfaces.Repositorios;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/FinTrack/[controller]")]
    public class TransacoesController(ITransacaoRepositorio transacaoRepositorio, ILogger<TransacoesController> logger) : ControllerBase
    {
        //Caso não tivesse primary constructor
        //private readonly ITransacaoRepositorio _transacaoRepositorio;
        //private readonly ILogger<TransacoesController> _logger;

        //public TransacoesController(ITransacaoRepositorio transacaoRepositorio, ILogger<TransacoesController> logger)
        //{
        //    _transacaoRepositorio = transacaoRepositorio;
        //    _logger = logger;
        //}

        private readonly ITransacaoRepositorio _transacaoRepositorio = transacaoRepositorio;
        private readonly ILogger<TransacoesController> _logger = logger;

        //Existe um EP chamado filtrarTransacoes que elimina boa parte destes EP
        //ele vai buscar de acordo com o que o usuário passar, se ele não passar nada
        //vai retornar tudo

        [HttpGet("obterTodas")]
        public async Task<ActionResult<IEnumerable<TransacaoContrato>>> ObterTodos()
        {
            try
            {
                var transacoes = await _transacaoRepositorio.ObterTodasAsync();
                var transacoesContrato = transacoes.Select(MapearParaContrato);

                return Ok(transacoesContrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações");
                return StatusCode(500, "Erro ao buscar transações");
            }
        }

        [HttpGet("obterPorId/{id}")]
        public async Task<ActionResult<TransacaoContrato>> ObterPorId(int id)
        {
            try
            {
                var transacao = await _transacaoRepositorio.ObterPorIdAsync(id);
                if (transacao == null) return NotFound($"Transação com id {id} não encontrada");

                return Ok(MapearParaContrato(transacao));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar transação {id}");
                return StatusCode(500, "Erro interno no servidor");
            }
        }

        [HttpGet("obterPorTipo/{tipoTransacao}")]
        public async Task<ActionResult<IEnumerable<TransacaoContrato>>> ObterPorTipo(TransacoesTipo tipoTransacao)
        {
            try
            {
                var transacoes = await _transacaoRepositorio.ObterPorTipoAsync(tipoTransacao);
                var transacoesContrato = transacoes.Select(MapearParaContrato);
                return Ok(transacoesContrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar transações pelo tipo {Enum.GetName(typeof(TransacoesTipo), tipoTransacao)}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("obterPorPeriodo")]
        public async Task<ActionResult<IEnumerable<TransacaoContrato>>> ObterPorPeriodo([FromQuery] DateTime dataInicio, [FromQuery] DateTime dataFim)
        {
            try
            {
                var transacoes = await _transacaoRepositorio.ObterPorPeriodoAsync(dataInicio, dataFim);
                var transacoesContrato = transacoes.Select(MapearParaContrato);
                return Ok(transacoesContrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao obter transações no perído: Inicio:{dataInicio}; Fim:{dataFim}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpGet("obterPorCategoriaId/{idCategoria}")]
        public async Task<ActionResult<IEnumerable<TransacaoContrato>>> ObterTransacaoPorCategoriaId(int idCategoria)
        {
            try
            {
                var transacoes = await _transacaoRepositorio.ObterPorCategoriaAsync(idCategoria);
                var transacaoContrato = transacoes.Select(MapearParaContrato);
                return Ok(transacaoContrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro ao buscar transações com a categoria {idCategoria}");
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost("obter")]
        public async Task<ActionResult<IEnumerable<TransacaoContrato>>> FiltrarTransacoes([FromBody] TransacaoFiltro filtros)
        {
            try
            {
                var transacoes = await _transacaoRepositorio.FiltrarTransacoes(filtros);
                var transacoesContrato = transacoes.Select(MapearParaContrato);

                return Ok(transacoesContrato);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar transações com os seguintes filtros: Id={Id}, Descricao={Descricao}, Tipo={Tipo}, CategoriaId={CategoriaId}, DataInicio={DataInicio}, DataFim={DataFim}",
                    filtros.Id,
                    filtros.Descricao,
                    filtros.Tipo,
                    filtros.CategoriaId,
                    filtros.DataInicio?.ToString("dd/MM/yyyy"),
                    filtros.DataFim?.ToString("dd/MM/yyyy"));
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPost("salvarTransacao")]
        public async Task<IActionResult> SalvarCategoria([FromBody] CriarTransacaoContrato contrato)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var transacao = new Transacao
                {
                    CategoriaId = contrato.CategoriaId,
                    Data = contrato.Data,
                    Descricao = contrato.Descricao,
                    Observacao = contrato.Observacao,
                    Tipo = contrato.Tipo,
                    Valor = contrato.Valor,
                };

                await _transacaoRepositorio.AdicionarAsync(transacao);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar transacao com os seguintes dados: Descricao={Descricao}, Valor={Valor}, Observacao={Observacao}, Tipo={Tipo}, CategoriaId={CategoriaId}, Data={Data}",
                    contrato.Descricao,
                    contrato.Valor,
                    contrato.Observacao,
                    contrato.Tipo,
                    contrato.CategoriaId,
                    contrato.Data.ToString("dd/MM/yyyy"));
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpDelete("removerTransacao/{id}")]
        public async Task<IActionResult> RemoverTransacao(int id)
        {
            try
            {
                var transacao = await _transacaoRepositorio.ObterPorIdAsync(id);
                if (transacao == null) return NotFound($"Transacao com o id: {id} não encontrada");

                await _transacaoRepositorio.DeletarAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao deletar transacao com id: " + id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        [HttpPut("editarTransacao")]
        public async Task<IActionResult> EditarTransacao([FromBody] AtualizarTransacaoContrato contrato)
        {
            try
            {
                if (!ModelState.IsValid) return BadRequest(ModelState);

                var transacao = await _transacaoRepositorio.ObterPorIdAsync(contrato.Id);
                if (transacao == null) return NotFound($"Transação com id ({contrato.Id}) não encontrada");

                transacao.Data = contrato.Data ?? transacao.Data;
                transacao.Observacao = contrato.Observacao ?? transacao.Observacao;
                transacao.Tipo = contrato.Tipo ?? transacao.Tipo;
                transacao.CategoriaId = contrato.CategoriaId ?? transacao.CategoriaId;
                transacao.Valor = contrato.Valor ?? transacao.Valor;
                transacao.Descricao = contrato.Descricao ?? transacao.Descricao;

                await _transacaoRepositorio.AtualizarAsync(transacao);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao editar categoria com id: " + contrato.Id);
                return StatusCode(500, "Erro interno do servidor");
            }
        }

        private static TransacaoContrato MapearParaContrato(Transacao transacao)
        {
            return new TransacaoContrato
            {
                CategoriaId = transacao.CategoriaId,
                Data = transacao.Data,
                Descricao = transacao.Descricao,
                Id = transacao.Id,
                NomeCategoria = transacao.Categoria.Nome,
                Observacao = transacao.Observacao,
                Tipo = transacao.Tipo,
                Valor = transacao.Valor
            };
        }
    }
}
