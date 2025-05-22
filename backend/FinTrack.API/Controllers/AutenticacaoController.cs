using FinTrack.Application.Contratos.Autenticacao;
using FinTrack.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/FinTrack/[controller]")]
    public class AutenticacaoController(IServicoAutenticacao autenticacaoServico, ILogger<AutenticacaoController> logger) : ControllerBase
    {
        private readonly IServicoAutenticacao _autenticacaoServico = autenticacaoServico;
        private readonly ILogger<AutenticacaoController> _logger = logger;

        [HttpPost("cadastrar")]
        [AllowAnonymous]
        public async Task<IActionResult> CadastrarUsuario([FromBody] CadastroUsuarioContrato cadastroUsuarioContrato)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (sucesso, mensagem) = await _autenticacaoServico.CadastrarUsuarioAsync(cadastroUsuarioContrato);
            if (!sucesso)
                return BadRequest(new { mensagem });

            return Ok(new { mensagem });
        }

        [HttpPost("logar")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginRespostaContrato>> Logar([FromBody] LogarContrato logarContrato)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var resposta = await _autenticacaoServico.LogarAsync(logarContrato);

            if (!resposta.Sucesso)
            {
                _logger.LogWarning("Falha no login para o usuário {Email}: {Message}", logarContrato.Email, resposta.Mensagem);
                return Unauthorized(new { message = resposta.Mensagem });
            }

            return Ok(resposta);
        }

        [HttpPost("recuperarSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> RecuperarSenha([FromBody] RecuperarSenhaContrato recuperarSenhaContrato)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (sucesso, mensagem) = await _autenticacaoServico.EsqueceuSenhaAsync(recuperarSenhaContrato);

            // Sempre retorne Ok por segurança, mesmo que ocorra um erro interno (já logado)
            // Isso evita que atacantes saibam quais emails existem ou não.
            _logger.LogInformation("Solicitação de esqueci minha senha processada para {Email}. Resultado: {Success}", recuperarSenhaContrato.Email, sucesso);
            return Ok(new { mensagem });
        }

        [HttpPost("resetarSenha")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetarSenha([FromBody] ResetarSenhaContrato resetarSenhaContrato)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (sucesso, mensagem) = await _autenticacaoServico.ResetarSenhaAsync(resetarSenhaContrato);

            if (!sucesso)
            {
                _logger.LogWarning("Falha ao resetar senha com token {Token}: {Message}", resetarSenhaContrato.Token, mensagem);
                return BadRequest(new { mensagem });
            }

            _logger.LogInformation("Senha resetada com sucesso usando token {Token}", resetarSenhaContrato.Token);
            return Ok(new { mensagem });
        }
    }
}
