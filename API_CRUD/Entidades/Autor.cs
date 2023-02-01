using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API_CRUD.Validaciones;

namespace API_CRUD.Entidades
{
    public class Autor
    {
        public int Id { get; set; }

        //Validaciones por atributos
        [Required(ErrorMessage ="El campo {0} es requerido")]
        [NombreCapital]
        [StringLength(maximumLength:100, ErrorMessage = "El campo {0} no puede exceder más de {1} caracteres")]
        public string Nombre { get; set; }

        //Propiedades de navegación
        public List<AutorLibro> AutoresLibros { get; set; }


    }
}
