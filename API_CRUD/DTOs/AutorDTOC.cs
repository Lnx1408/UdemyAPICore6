using API_CRUD.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace API_CRUD.DTOs
{
    public class AutorDTOC
    {
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [NombreCapital]
        [StringLength(maximumLength: 100, ErrorMessage = "El campo {0} no puede exceder más de {1} caracteres")]
        public string Nombre { get; set; }
        
    }
}
