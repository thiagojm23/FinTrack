using System.ComponentModel.DataAnnotations;

namespace FinTrack.Application.Contratos.Autenticacao
{
    public class CadastroUsuarioContrato
    {
        [Required(ErrorMessage = "Nome completo obrigatório")]
        [MaxLength(150)]
        public string NomeCompleto { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email obrigatório")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Senha obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        public string Senha { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmação de senha obrigatória")]
        [Compare("Senha", ErrorMessage = "As senhas não coincidem")]
        public string ConfirmarSenha { get; set; } = string.Empty;
    }
}
