using Microsoft.AspNetCore.Mvc.Filters;

namespace API_CRUD.Filtros
{

    //Filtro personalizado que se va a usar de manera global
    public class FiltroExepcion : ExceptionFilterAttribute
    {
        private readonly ILogger<FiltroExepcion> logger;

        public FiltroExepcion(ILogger<FiltroExepcion> logger)
        {
            this.logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            logger.LogError(context.Exception, context.Exception.Message);
        }
    }
}
