namespace FinTrack.Application.Interfaces
{
    public interface IServicoEmail
    {
        Task<bool> EnviarEmailAsync(string emailDestino, string assuntoEmail, string conteudoEmail);
    }
}
