using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API_CRUD.Utilidades
{
    /// <summary>
    /// Implementar los filtros para generar los enlaces de la API - Clase base
    /// </summary>
    public class HATEOASFiltroAttribute: ResultFilterAttribute
    {
        protected bool debeIncluirHATEOAS(ResultExecutingContext context)
        {
            var resultado = context.Result as ObjectResult;
            if(!esRespuestaExitosa(resultado))
            {
                return false;
            }

            var cabecera = context.HttpContext.Request.Headers["incluirHATEOAS"];

            if(cabecera==0)
            {
                return false;
            }

            var valor = cabecera[0];
            if(!valor.Equals("Y", StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            return true;

        }
        private bool esRespuestaExitosa(ObjectResult resultado)
        {
            if(resultado == null || resultado.Value==null)
            {
                return false;
            }

            if(resultado.StatusCode.HasValue && !resultado.StatusCode.Value.ToString().StartsWith("2"))
            {
                return false;
            }
            return true;
        }
    }
}
