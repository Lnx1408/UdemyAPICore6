using API_CRUD.Entidades;

namespace API_CRUD.DTOs
{
    public class LibroDTOR: Recurso
    {
        public int Id { get; set; }
        public string Titulo { get; set; }

        


        /*
         * Traer los comentarios cuando se cargue los libros
         * public List<ComentarioDTOR> Comentarios { get; set; }
        */
    }
}
