using System.ComponentModel.DataAnnotations;

namespace FinTrack.Application.Contratos.Autenticacao
{
    public class ResetarSenhaContrato
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve conter no mínimo 6 caracteres")]
        public string NovaSenha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha obrigatório")]
        [Compare("NovaSenha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarNovaSenha { get; set; } = string.Empty;
    }
}
