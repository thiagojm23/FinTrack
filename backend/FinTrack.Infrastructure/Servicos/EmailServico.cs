using FinTrack.Application.Interfaces;
using FinTrack.Infrastructure.Configs;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace FinTrack.Infrastructure.Servicos
{
    public class EmailServico(IOptions<EmailSettings> emailConfigs, ILogger<EmailServico> logger) : IServicoEmail
    {
        private readonly ILogger<EmailServico> _logger = logger;
        private readonly EmailSettings _emailConfigs = emailConfigs.Value;
        public async Task<bool> EnviarEmailAsync(string emailDestino, string assuntoEmail, string conteudoEmail)
        {
            if (new[] { _emailConfigs.SmtpServer, _emailConfigs.SmtpPass, _emailConfigs.SmtpUser, _emailConfigs.SenderEmail }.Any(string.IsNullOrEmpty))
            {
                _logger.LogWarning("Banana {pass}", _emailConfigs.SmtpPass);
                _logger.LogWarning("Banana {user}", _emailConfigs.SmtpUser);
                _logger.LogError("Configurações de SMTP não estão completas. Não é possível enviar email.");
                return false;
            }
            try
            {
                var mensagemEmail = new MimeMessage();
                mensagemEmail.From.Add(new MailboxAddress(_emailConfigs.SenderName, _emailConfigs.SenderEmail));
                mensagemEmail.To.Add(MailboxAddress.Parse(emailDestino));
                mensagemEmail.Subject = assuntoEmail;
                mensagemEmail.Body = new TextPart(TextFormat.Html) { Text = conteudoEmail };

                using var smtpClient = new SmtpClient();
                // Conectar de forma segura. MailKit tentará STARTTLS se disponível.
                // Para SSL/TLS na porta 465, use smtpClient.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.SmtpPort, true);
                // Para STARTTLS na porta 587 (mais comum), use SecureSocketOptions.StartTls.
                // StartTlsWhenAvailable tenta STARTTLS, mas pode voltar para uma conexão não segura se não for suportado (menos ideal).
                // É melhor ser explícito com base na configuração do seu servidor.
                await smtpClient.ConnectAsync(_emailConfigs.SmtpServer, _emailConfigs.SmtpPort, SecureSocketOptions.StartTlsWhenAvailable);

                // Autenticar
                // Alguns servidores podem não exigir autenticação se o envio for de IPs confiáveis (raro).
                await smtpClient.AuthenticateAsync(_emailConfigs.SmtpUser, _emailConfigs.SmtpPass);

                await smtpClient.SendAsync(mensagemEmail);
                await smtpClient.DisconnectAsync(true);

                _logger.LogInformation("Email enviado com sucesso para {Email} com o conteudo: {Assunto}", emailDestino, assuntoEmail);
                return true;
            }
            catch (SmtpCommandException ex)
            {
                _logger.LogError(ex, "Erro de comando SMTP ao enviar email para {ToEmail}. StatusCode: {StatusCode}, Response: {Response}",
                                 emailDestino, ex.StatusCode, ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro genérico ao enviar email para {Email} com assunto '{Assunto}'", emailDestino, assuntoEmail);
                // Relançar pode ser apropriado dependendo da sua estratégia de tratamento de erros.
                // throw; // Descomente se quiser que a exceção se propague
                return false;
            }
        }
    }
}
