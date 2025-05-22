using FinTrack.Domain.Entities;
using FinTrack.Domain.Enums;
using FinTrack.Domain.Filtros;

namespace FinTrack.Domain.Interfaces.Repositorios
{
    public interface ITransacaoRepositorio : IBaseRepositorio<Transacao>
    {
        Task<IEnumerable<Transacao>> ObterPorPeriodoAsync(DateTime dataInicio, DateTime dataFim);
        Task<IEnumerable<Transacao>> ObterPorTipoAsync(TransacoesTipo tipo);
        Task<IEnumerable<Transacao>> ObterPorCategoriaAsync(int categoriaId);
        Task<IEnumerable<Transacao>> FiltrarTransacoes(TransacaoFiltroDominio filtros);
    }
}
