using System.Linq.Expressions;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Interfaces.Repositorios;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositorios
{
    public class RepositorioBase<T> : IBaseRepositorio<T> where T : EntidadeBase
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public RepositorioBase(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> ObterPorIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> ObterTodasAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<T>> EncontrarAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task AdicionarAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeletarAsync(int id)
        {
            var entity = await ObterPorIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
