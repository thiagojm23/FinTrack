namespace FinTrack.Application.Contratos.Autenticacao
{
    public class LoginRespostaContrato
    {
        public bool Sucesso { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public string? Token { get; set; }
        public DateTime? ExpiracaoToken { get; set; }
        public InformacoesUsuario Usuario { get; set; }
    }

    public class InformacoesUsuario
    {
        public int Id { get; set; }
        public string NomeCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}
