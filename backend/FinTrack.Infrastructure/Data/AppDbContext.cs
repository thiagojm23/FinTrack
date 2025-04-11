using FinTrack.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinTrack.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Transacao> Transacoes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Config de transação
            modelBuilder.Entity<Transacao>().Property(t => t.Valor).HasPrecision(18, 2);

            //Config de transação
            modelBuilder.Entity<Transacao>().Property(c => c.Descricao).IsRequired().HasMaxLength(100);

            //Config de categoria
            modelBuilder.Entity<Categoria>().Property(c => c.Nome).IsRequired().HasMaxLength(50);

            //Seed categorias padrão
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nome = "Renda recorrente", Descricao = "Receitas de salário", DataCriacao = DateTime.UtcNow },
                new Categoria { Id = 2, Nome = "Alimentação", Descricao = "Gastos com alimentação", DataCriacao = DateTime.UtcNow },
                new Categoria { Id = 3, Nome = "Transporte", Descricao = "Gastos com transporte", DataCriacao = DateTime.UtcNow },
                new Categoria { Id = 4, Nome = "Moradia", Descricao = "Gastos com moradia", DataCriacao = DateTime.UtcNow },
                new Categoria { Id = 5, Nome = "Lazer", Descricao = "Gastos com lazer", DataCriacao = DateTime.UtcNow },
                new Categoria { Id = 6, Nome = "Renda passiva", Descricao = "Renda proveniente de investimentos em geral", DataCriacao = DateTime.UtcNow }
            );

        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entidade in ChangeTracker.Entries<EntidadeBase>())
            {
                if (entidade.State == EntityState.Modified)
                {
                    entidade.Entity.DataAtualizacao = DateTime.UtcNow;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

    }
}
