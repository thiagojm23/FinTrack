using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FinTrack.Application.Contratos.Autenticacao;
using FinTrack.Application.Interfaces;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Interfaces.Repositorios;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace FinTrack.Application.Services
{
    public class AutenticacaoServico(IUsuarioRepositorio usuarioRepositorio, IConfiguration configuration, IServicoEmail emailService, ILogger<AutenticacaoServico> logger) : IServicoAutenticacao
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio = usuarioRepositorio;
        private readonly IConfiguration _configuration = configuration;
        private readonly IServicoEmail _emailService = emailService;
        private readonly ILogger<AutenticacaoServico> _logger = logger;

        public async Task<(bool Sucesso, string Mensagem)> CadastrarUsuarioAsync(CadastroUsuarioContrato usuario)
        {
            try
            {
                var usuarioCadastrado = await _usuarioRepositorio.ObterPorEmailAsync(usuario.Email);
                if (usuarioCadastrado != null) return (false, "Email já cadastrado");

                var novoUsuario = new Usuario
                {
                    NomeCompleto = usuario.NomeCompleto,
                    Email = usuario.Email,
                    SenhaHash = BCrypt.Net.BCrypt.HashPassword(usuario.Senha)
                };

                await _usuarioRepositorio.AdicionarAsync(novoUsuario);
                return (true, "Usuário cadastrado com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar usuário {Email}", usuario.Email);
                return (false, "Ocorreu um erro durante o registro");
            }
        }

        public async Task<LoginRespostaContrato> LogarAsync(LogarContrato dadosLogin)
        {
            try
            {
                var usuario = await _usuarioRepositorio.ObterPorEmailAsync(dadosLogin.Email);
                if (usuario == null || !BCrypt.Net.BCrypt.Verify(dadosLogin.Senha, usuario.SenhaHash))
                    return new LoginRespostaContrato { Sucesso = false, Mensagem = "Email ou senha inválidos" };

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("Chave JWT não configurada"));
                var expiracaoToken = DateTime.UtcNow.AddHours(1);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(
                    [
                        new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                        new(ClaimTypes.Email, usuario.Email),
                        new(ClaimTypes.Name, usuario.NomeCompleto),
                    ]),
                    Expires = expiracaoToken,
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return new LoginRespostaContrato
                {
                    ExpiracaoToken = token.ValidTo,
                    Mensagem = "Login feito com sucesso",
                    Sucesso = true,
                    Token = tokenString,
                    Usuario = new InformacoesUsuario { NomeCompleto = usuario.NomeCompleto, Id = usuario.Id, Email = usuario.Email }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao logar usuário {Email}", dadosLogin.Email);
                return new LoginRespostaContrato { Sucesso = false, Mensagem = "Ocorreu um erro durante o login" };

            }
        }

        public async Task<(bool Sucesso, string Mensagem)> EsqueceuSenhaAsync(RecuperarSenhaContrato recuperarSenhaContrato)
        {
            try
            {
                var usuario = await _usuarioRepositorio.ObterPorEmailAsync(recuperarSenhaContrato.Email);
                if (usuario == null)
                {
                    _logger.LogError("Email não encontrado no banco: {Email}", recuperarSenhaContrato.Email);
                    return (true, "Se um usuário com este email existir, um link para redefinição de senha foi enviado.");
                }

                usuario.TokenResetarSenha = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
                usuario.ExpiracaoResetToken = DateTime.UtcNow.AddHours(1);

                await _usuarioRepositorio.AtualizarAsync(usuario);

                var linkTelaResetarSenha = $"https://localhost:5173/reset-password?token={usuario.TokenResetarSenha}";
                var mensagem = $"<p>Por favor, clique no link abaixo para redefinir sua senha:</p><p><a href='{linkTelaResetarSenha}'>Redefinir Senha</a></p><p>Este link expira em 1 hora.</p>";

                await _emailService.EnviarEmailAsync(usuario.Email, "Redefinir senha - FinTrack", mensagem);

                return (true, "Se um usuário com este email existir, um link para redefinição de senha foi enviado.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar email de redefinição de senha para usuário {Email}", recuperarSenhaContrato.Email);
                return (false, "Ocorreu um erro ao processar sua solicitação");
            }
        }

        public async Task<(bool Sucesso, string Mensagem)> ResetarSenhaAsync(ResetarSenhaContrato dadosResetarSenha)
        {
            try
            {
                var usuario = await _usuarioRepositorio.ObterPorTokenResetSenhaAsync(dadosResetarSenha.Token);
                if (usuario == null || usuario.ExpiracaoResetToken == null || usuario.ExpiracaoResetToken < DateTime.UtcNow)
                    return (false, "Token inválido ou expirado");

                usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(dadosResetarSenha.NovaSenha);
                usuario.TokenResetarSenha = null;
                usuario.ExpiracaoResetToken = null;
                usuario.DataResetSenha = DateTime.UtcNow;

                await _usuarioRepositorio.AtualizarAsync(usuario);

                await _emailService.EnviarEmailAsync(usuario.Email, "Sua senha foi alterada - FinTrack", "Sua senha na plataforma FinTrack foi alterada com sucesso.");

                return (true, "Senha alterada com sucesso");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao tentar resetar senha com token: {token}", dadosResetarSenha.Token);
                return (false, "Erro ao resetar senha");
            }
        }
    }
}
