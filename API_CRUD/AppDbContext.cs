using Microsoft.EntityFrameworkCore;
using API_CRUD.Entidades;

namespace API_CRUD
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //Construir una llave compuesta 
            modelBuilder.Entity<AutorLibro>().HasKey(autorLibro => new {autorLibro.AutorId, autorLibro.LibroId});
        }

        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }

        public DbSet<Comentario> Comentarios { get; set; }

        public DbSet<AutorLibro> AutoresLibros { get; set;}
    }
}
