using FinTrack.Domain.Enums;
namespace FinTrack.Application.Contratos
{
    public class TransacaoFiltroContrato
    {
        public int? Id { get; set; }
        public string? Descricao { get; set; }
        public decimal? ValorMaximo { get; set; }
        public decimal? ValorMinimo { get; set; }
        public DateTime? DataInicio { get; set; }
        public DateTime? DataFim { get; set; }
        public TransacoesTipo? Tipo { get; set; }
        public int? CategoriaId { get; set; }
        public string? NomeCategoria { get; set; }
        public string? Observacao { get; set; }
    }
}
