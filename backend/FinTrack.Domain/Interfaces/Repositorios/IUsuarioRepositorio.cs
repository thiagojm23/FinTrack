using FinTrack.Domain.Entities;

namespace FinTrack.Domain.Interfaces.Repositorios
{
    public interface IUsuarioRepositorio : IBaseRepositorio<Usuario>
    {
        Task<Usuario?> ObterPorEmailAsync(string email);
        Task<Usuario?> ObterPorTokenResetSenhaAsync(string token);
    }
}
