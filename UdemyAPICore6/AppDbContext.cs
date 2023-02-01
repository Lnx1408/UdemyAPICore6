using Microsoft.EntityFrameworkCore;
using UdemyAPICore6.Entidades;

namespace UdemyAPICore6
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Autor> Autores { get; set; }
        public DbSet<Libro> Libros { get; set; }
    }
}
