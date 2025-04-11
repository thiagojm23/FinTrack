using System.Linq.Expressions;
using FinTrack.Domain.Entities;

namespace FinTrack.Domain.Interfaces.Repositorios
{
    public interface IBaseRepositorio<T> where T : EntidadeBase
    {
        Task<T?> ObterPorIdAsync(int id);
        Task<IEnumerable<T>> ObterTodasAsync();
        Task<IEnumerable<T>> EncontrarAsync(Expression<Func<T, bool>> predicate);
        Task AdicionarAsync(T entity);
        Task AtualizarAsync(T entity);
        Task DeletarAsync(int id);
    }
}
