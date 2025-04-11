using FinTrack.Domain.Contratos;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Domain.Interfaces.Repositorios;
using FinTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Repositorios
{
    public class TransacaoRepositorio(AppDbContext context) : RepositorioBase<Transacao>(context), ITransacaoRepositorio
    {
        //Caso não fosse usado primary constructor
        //public TransacaoRepositorio(AppDbContext context) : base(context)
        //{
        //}

        public async Task<IEnumerable<Transacao>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim)
        {
            return await _dbSet.Include(t => t.Categoria).Where(t => t.Data >= dataInicio && t.Data <= dataFim).OrderByDescending(t => t.Data).ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterPorTipoAsync(TransacoesTipo tipo)
        {
            return await _dbSet.Include(t => t.Categoria).Where(t => t.Tipo == tipo).OrderByDescending(t => t.Data).ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> ObterPorCategoriaAsync(int categoriaId)
        {
            return await _dbSet.Include(t => t.Categoria).Where(t => t.CategoriaId == categoriaId).OrderByDescending(t => t.Data).ToListAsync();
        }

        public async Task<IEnumerable<Transacao>> FiltrarTransacoes(TransacaoFiltro filtros)
        {
            var query = _dbSet.Include(t => t.Categoria).AsQueryable();

            query = query.Where(t => !filtros.DataInicio.HasValue || t.Data >= filtros.DataInicio)
                .Where(t => !filtros.DataFim.HasValue || t.Data <= filtros.DataFim)
                .Where(t => !filtros.ValorMaximo.HasValue || t.Valor <= filtros.ValorMaximo)
                .Where(t => !filtros.ValorMinimo.HasValue || t.Valor <= filtros.ValorMinimo)
                .Where(t => !filtros.Tipo.HasValue || t.Tipo == filtros.Tipo)
                .Where(t => !filtros.CategoriaId.HasValue || t.CategoriaId == filtros.CategoriaId)
                .Where(t => !filtros.Id.HasValue || t.Id == filtros.Id)
                .Where(t => string.IsNullOrEmpty(filtros.NomeCategoria) || t.Categoria.Nome.Contains(filtros.NomeCategoria.Trim(), StringComparison.OrdinalIgnoreCase))
                .Where(t => string.IsNullOrEmpty(filtros.Observacao) || t.Observacao.Contains(filtros.Observacao.Trim(), StringComparison.OrdinalIgnoreCase));

            return await query.OrderByDescending(t => t.Data).ToListAsync();
        }

        public override async Task<Transacao?> ObterPorIdAsync(int id)
        {
            return await _dbSet.Include(t => t.Categoria).FirstOrDefaultAsync(t => t.Id == id);
        }

        public override async Task<IEnumerable<Transacao>> ObterTodasAsync()
        {
            return await _dbSet.Include(t => t.Categoria).OrderByDescending(t => t.Data).ToListAsync();
        }
    }
}
