using System.ComponentModel.DataAnnotations;

namespace FinTrack.Application.Contratos.Autenticacao
{
    public class RecuperarSenhaContrato
    {
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;
    }
}
