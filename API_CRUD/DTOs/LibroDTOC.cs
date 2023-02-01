using API_CRUD.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace API_CRUD.DTOs
{
    public class LibroDTOC
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [NombreCapital]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede exceder más de {1} caracteres")]
        public string Titulo { get; set; }

        public List<int> AutoresIds { get; set; }
    }
}
