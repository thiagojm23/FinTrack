using System.ComponentModel.DataAnnotations;

namespace FinTrack.Application.Contratos.Autenticacao
{
    public class LogarContrato
    {
        [Required(ErrorMessage = "Email obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha obrigatória")]
        public string Senha { get; set; } = string.Empty;
    }
}
