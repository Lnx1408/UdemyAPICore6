using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UdemyAPICore6.Validaciones;

namespace UdemyAPICore6.Entidades
{
    public class Autor
    {
        public Autor() { }


        public int Id { get; set; }

        //Validaciones por atributos
        [Required(ErrorMessage ="El campo {0} es requerido")] //Colocando {0} hace que se reemplace por el nombre del atributo
        [NombreCapital]
        [StringLength(maximumLength:10, ErrorMessage = "El campo {0} no puede exceder más de {1} caracteres")]
        public string Nombre { get; set; }
        public List<Libro> Libros { get; set; }



        //[Range(18, 100, ErrorMessage ="El campo {0} debe estar dentro del rango [{1}-{2}]")]
        //[NotMapped]
        //public int Edad { get; set; }

        //[CreditCard]
        //[NotMapped]
        //public string TarjetaCredito { get; set; }

        //[Url]
        //[NotMapped]
        //public string Url { get; set; }
        
    }
}
