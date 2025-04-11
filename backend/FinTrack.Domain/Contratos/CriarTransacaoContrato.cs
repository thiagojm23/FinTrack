using System.ComponentModel.DataAnnotations;
using FinTrack.Domain.Enums;

namespace FinTrack.Domain.Contratos
{
    public class CriarTransacaoContrato
    {
        [Required(ErrorMessage = "Descrição é obrigatória", AllowEmptyStrings = false)]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Valor é obrigatório")]
        public decimal Valor { get; set; }

        [Required(ErrorMessage = "Data é obrigatória")]
        public DateTime Data { get; set; }

        [Required(ErrorMessage = "Tipo é obrigatório")]
        public TransacoesTipo Tipo { get; set; }

        [Required(ErrorMessage = "Categoria é obrigatória")]
        public int CategoriaId { get; set; }
        public string? Observacao { get; set; }
    }
}
