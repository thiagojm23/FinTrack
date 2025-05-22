using FinTrack.Application.Contratos.Autenticacao;

namespace FinTrack.Application.Interfaces
{
    public interface IServicoAutenticacao
    {
        Task<(bool Sucesso, string Mensagem)> CadastrarUsuarioAsync(CadastroUsuarioContrato usuario);
        Task<LoginRespostaContrato> LogarAsync(LogarContrato dadosLogin);
        Task<(bool Sucesso, string Mensagem)> EsqueceuSenhaAsync(RecuperarSenhaContrato recuperarSenhaContrato);
        Task<(bool Sucesso, string Mensagem)> ResetarSenhaAsync(ResetarSenhaContrato dadosResetarSenha);
    }
}
