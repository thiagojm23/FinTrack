namespace FinTrack.Domain.Entities
{
    public class Categoria : EntidadeBase
    {
        public string Nome { get; set; }
        public string? Descricao { get; set; }
        public ICollection<Transacao> Transacoes { get; set; }
    }
}
