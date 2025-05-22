using FinTrack.Application.Contratos;
using FinTrack.Domain.Entities;
using FinTrack.Domain.Filtros;
using FinTrack.Domain.Interfaces.Repositorios;

namespace FinTrack.Application.Services
{
    public class TransacaoApplicationServico(ITransacaoRepositorio transacaoRepositorio)
    {
        private readonly ITransacaoRepositorio _transacaoRepositorio = transacaoRepositorio;

        public async Task<IEnumerable<Transacao>> FiltrarTransacoes(TransacaoFiltroContrato filtros)
        {
            var filtrosDominio = new TransacaoFiltroDominio
            {
                Id = filtros.Id,
                Descricao = filtros.Descricao,
                DataInicio = filtros.DataInicio,
                DataFim = filtros.DataFim,
                ValorMinimo = filtros.ValorMinimo,
                ValorMaximo = filtros.ValorMaximo,
                Tipo = filtros.Tipo,
                CategoriaId = filtros.CategoriaId,
                NomeCategoria = filtros.NomeCategoria,
                Observacao = filtros.Observacao
            };

            return await _transacaoRepositorio.FiltrarTransacoes(filtrosDominio);
        }
    }
}
