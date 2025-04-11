namespace FinTrack.Domain.Entities
{
    public class EntidadeBase
    {
        public int Id { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
        public DateTime DataAtualizacao { get; set; }

    }
}
