using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Contratos
{
    public class CriarCategoriaContrato
    {

        [Required(ErrorMessage = "Necessário informar um nome para a categoria")]
        public string Nome { get; set; }
        public string? Descricao { get; set; }
    }
}
