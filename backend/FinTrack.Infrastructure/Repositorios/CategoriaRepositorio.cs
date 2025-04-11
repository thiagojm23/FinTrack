using FinTrack.Domain.Entities;
using FinTrack.Domain.Interfaces.Repositorios;
using FinTrack.Infrastructure.Data;

namespace FinTrack.Infrastructure.Repositorios
{
    public class CategoriaRepositorio(AppDbContext context) : RepositorioBase<Categoria>(context), ICategoriaRepositorio
    {
        //Caso não fosse usado primary constructor
        //public CategoriaRepositorio(AppDbContext context) : base(context)
        //{
        //}
    }
}
