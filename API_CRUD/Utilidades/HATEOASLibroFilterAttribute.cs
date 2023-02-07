using API_CRUD.DTOs;
using API_CRUD.Servicios;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API_CRUD.Utilidades
{
    public class HATEOASLibroFilterAttribute: HATEOASFiltroAttribute
    {
        private readonly GeneradorEnlaces generadorEnlaces;

        public HATEOASLibroFilterAttribute(GeneradorEnlaces generadorEnlaces)
        {
            this.generadorEnlaces = generadorEnlaces;
        }
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {


            var debeIncluir = debeIncluirHATEOAS(context);
            if (!debeIncluir)
            {
                await next();
                return;
            }


            var resultado = context.Result as ObjectResult;

            var modelo = resultado.Value as LibroDTOR ?? throw new ArgumentNullException("Se esperaba una instancia de Autores");


            await generadorEnlaces.GenerarEnlaces(modelo);
            await next();

        }

    }
}
