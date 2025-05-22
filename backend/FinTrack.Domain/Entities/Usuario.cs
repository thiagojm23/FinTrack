using System.ComponentModel.DataAnnotations;

namespace FinTrack.Domain.Entities
{
    public class Usuario : EntidadeBase
    {
        [Required]
        [MaxLength(150)]
        public string NomeCompleto { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string SenhaHash { get; set; } = string.Empty;

        public string? TokenResetarSenha { get; set; }
        public DateTime? ExpiracaoResetToken { get; set; }
        public DateTime? DataResetSenha { get; set; }

    }
}
