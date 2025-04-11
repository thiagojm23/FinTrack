using System.ComponentModel.DataAnnotations;
using FinTrack.Domain.Enums;

namespace FinTrack.Domain.Contratos
{
    public class AtualizarTransacaoContrato
    {
        [Required(ErrorMessage = "Id obrigatório")]
        public int Id { get; set; }
        public string? Descricao { get; set; }
        public decimal? Valor { get; set; }
        public DateTime? Data { get; set; }
        public TransacoesTipo? Tipo { get; set; }
        public int? CategoriaId { get; set; }
        public string? Observacao { get; set; }
    }
}
