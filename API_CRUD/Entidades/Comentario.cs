using Microsoft.AspNetCore.Identity;

namespace API_CRUD.Entidades
{
    public class Comentario
    {
        public int Id { get; set; }
        public string Contenido { get; set; }
        public int LibroId { get; set; }

        //Popiedad de navegación: Facilita pasar de una entidad a otra relacionada (Facilita el uso de JOINS)
        public Libro Libro { get; set; }
        public string UsuarioId { get; set;}
        public IdentityUser Usuario { get; set;}
    }
}