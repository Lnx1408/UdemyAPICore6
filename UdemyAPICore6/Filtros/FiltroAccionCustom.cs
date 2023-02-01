using Microsoft.AspNetCore.Mvc.Filters;
using UdemyAPICore6.Middlewares;

namespace UdemyAPICore6.Filtros
{
    public class FiltroAccionCustom : IActionFilter
    {
        private readonly ILogger<FiltroAccionCustom> logger;

        public FiltroAccionCustom(ILogger<FiltroAccionCustom> logger) 
        {
            this.logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {

            logger.LogInformation("Después de ejecutar la acción");
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            logger.LogInformation("Antes de ejecutar la acción");
        }
    }


    public static class FiltroAccionCustomExtensions
    {
        public static IServiceCollection AddFiltroAccionCustom(this IServiceCollection services)
        {
            return services.AddTransient<FiltroAccionCustom>();
        }
    }
}
