using System.ComponentModel.DataAnnotations;

namespace UdemyAPICore6.Entidades
{

    //Validaciones por objetos
    public class Libro: IValidatableObject
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public int AutorId { get; set;}
        public Autor Autor { get; set; }

        //public int Mayor { get; set; }
        //public int Menor { get; set; }


        /// <summary>
        /// Para que se ejecuten estas reglas de validación por Modelo, primero deben resolverse las reglas de validación por atributo
        /// </summary>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(!string.IsNullOrEmpty(Titulo))
            {
                var primeraLetra = Titulo[0].ToString();
                if(primeraLetra != primeraLetra.ToUpper())
                {
                    yield return new ValidationResult("La primera letra debe ser mayúscula", new string[] {nameof(Titulo)});
                }
            }

            //if(Menor > Mayor)
            //{
            //    yield return new ValidationResult("Este valor no puede ser mayor que el campo Mayor", new string[] {nameof(Menor)});

            //}
        }


    }
}
