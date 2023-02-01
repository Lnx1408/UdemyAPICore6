using API_CRUD.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace API_CRUD.Entidades
{

    //Validaciones por objetos
    public class Libro
    {
        public int Id { get; set; }


        [NombreCapital]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede exceder más de {1} caracteres")]
        public string Titulo { get; set; }
        public DateTime? FechaPublicacion { get; set; }

        //Atributo de navegación
        public List<Comentario> Comentarios { get; set; }

        public List<AutorLibro> AutoresLibros { get; set;}

    }
}
