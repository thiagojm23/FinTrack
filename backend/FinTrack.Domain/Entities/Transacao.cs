using FinTrack.Domain.Enums;

namespace FinTrack.Domain.Entities
{
    public class Transacao : EntidadeBase
    {
        public string Descricao { get; set; }
        public decimal Valor { get; set; }
        public DateTime Data { get; set; }
        public TransacoesTipo Tipo { get; set; }
        public int? CategoriaId { get; set; }
        public virtual Categoria Categoria { get; set; }
        public string? Observacao { get; set; }
    }
}