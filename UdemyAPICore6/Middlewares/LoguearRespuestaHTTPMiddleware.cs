using Microsoft.Extensions.Logging;

namespace UdemyAPICore6.Middlewares
{
    /// <summary>
    /// Clase del middleware personalizado
    /// </summary>
    public class LoguearRespuestaHTTPMiddleware
    {
        private readonly RequestDelegate siguiente;
        private readonly ILogger<LoguearRespuestaHTTPMiddleware> logger;

        /// <summary>
        /// Constructor de la clase del middleware
        /// </summary>
        /// <param name="siguiente">Permite invocar los siguientes middleware de la tubería</param>
        public LoguearRespuestaHTTPMiddleware(RequestDelegate siguiente, ILogger<LoguearRespuestaHTTPMiddleware> logger)
        {
            this.siguiente = siguiente;
            this.logger = logger;
        }

        //Invoke o InvokeAsync
        public async Task InvokeAsync(HttpContext contexto)
        {
            using (var ms = new MemoryStream())
            {
                //Guardar en memoria la respuesta de la petición http
                var cuerpoOriginalRespuesta = contexto.Response.Body;
                contexto.Response.Body = ms;

                //Nota: Lo que venga luego de este await se va a ejecutar luego de que los middleware posteriores hayan dado una respuesta
                await siguiente(contexto);

                //Se ubica en la posicion inicial del stream
                ms.Seek(0, SeekOrigin.Begin);
                //Guarda lo que se le va a enviar al cliente en un String y enviarla al log
                string respuesta = new StreamReader(ms).ReadToEnd();



                ms.Seek(0, SeekOrigin.Begin);
                //Devolver la respuesta inicial para que el usuario pueda utilizarla
                await ms.CopyToAsync(cuerpoOriginalRespuesta);
                contexto.Response.Body = cuerpoOriginalRespuesta;

                logger.LogInformation(respuesta);


            }

        }
    }

    /// <summary>
    /// Clase estática usada para ocultar la clase que se está usando cuando se llama al middleware, ya que se accede directamente a su método.
    /// </summary>
    public static class LoguearRespuestaHTTPMiddlewareExtensions
    {
        public static IApplicationBuilder UseLoguearRespuestaHTTP(this IApplicationBuilder app)
        {
            return app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
        }
    }
}
