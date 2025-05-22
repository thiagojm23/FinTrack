using FinTrack.Domain.Entities;
using FinTrack.Domain.Interfaces.Repositorios;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositorios
{
    public class UsuarioRepositorio(AppDbContext context) : RepositorioBase<Usuario>(context), IUsuarioRepositorio
    {
        public async Task<Usuario?> ObterPorEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<Usuario?> ObterPorTokenResetSenhaAsync(string token)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.TokenResetarSenha == token);
        }

        public override async Task AdicionarAsync(Usuario usuario)
        {
            usuario.Email = usuario.Email.ToLowerInvariant();
            await base.AdicionarAsync(usuario);
        }
    }
}
