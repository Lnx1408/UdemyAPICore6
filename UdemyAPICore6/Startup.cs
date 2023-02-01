using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using UdemyAPICore6.Filtros;
using UdemyAPICore6.Middlewares;
using UdemyAPICore6.Servicios;

namespace UdemyAPICore6
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }


        /// <summary>
        /// Este método lo complementamos con instrucciones de la clase program.
        /// El método permite solventar las dependencias, aquí se las puede configurar y así usarla en cualquier parte del programa.
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            //Configuración de IServicio creado para resolver las dependencias.
            services.AddTransient<IServicio, ServicioA>();
            
            //Ejemplos del ciclo de vida de un servicio
            services.AddTransient<ServicioTransient>();
            services.AddScoped<ServicioScope>();
            services.AddSingleton<ServicioSingleton>();
            services.AddFiltroAccionCustom();

            //Servicio de filtro de caché
            services.AddResponseCaching();

            //Servicio IHostedService para escribir en archivo
            services.AddHostedService<EscribirArchivo>();

            //Servicio de filtro de autorización - Instalar el paquete NuGet "Microsoft.AspNetCore.Authentication.JwtBearer"
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddControllers(opciones =>
            {
                //Añadir el filtro de manera global
                opciones.Filters.Add(typeof(FiltroExepcion));

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            services.AddDbContext<AppDbContext>(options=> options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();


        }


        /// <summary>
        /// Este método lo complementamos con instrucciones de la clase program.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            //Nota: El inicio del método es el lugar ideal para obtener las respuesta final antes de ser enviadas al cliente.

            /*
            //MiddleWare personalizado - Guardar en log las peticiones de los usuarios (Uso desde la clase Startup).
            app.Use(async (contexto, siguiente) =>
            {
                using (var ms = new MemoryStream())
                {
                    //Guardar en memoria la respuesta de la petición http
                    var cuerpoOriginalRespuesta = contexto.Response.Body;
                    contexto.Response.Body = ms;

                    //Nota: Lo que venga luego de este await se va a ejecutar luego de que los middleware posteriores hayan dado una respuesta
                    await siguiente.Invoke();

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
            });

            */

            /*
             Formas de utilizar el middleware de la clase "LoguearRespuestaHTTPMiddleware"

            1ra Forma
            
            app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            */


            //2da Forma: Se accede directamente al método de la clase estática. 
            app.UseLoguearRespuestaHTTP();

            //Con este middleware lo que hacemos en que si la ruta que se especifica es "/ruta-fantasma", entonces nos muestra el mensaje de respuesta
            app.Map("/ruta-fantasma", app =>
            {
                app.Run(async contexto =>
                {
                    await contexto.Response.WriteAsync("Se ha interseptado la ejecución de la API");
                });

            });




            
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //Si se coloca al inicio no carga el swagger, pero en esta posición sí se ejecuta, pero no hay respuesta en las APIS
            //app.Run(async contexto =>
            //{
            //    await contexto.Response.WriteAsync("Se ha interseptado la ejecución de la API");
            //});

            app.UseRouting();

            //Filtro de recurso
            app.UseResponseCaching();

            //Permite usar el filtro de autorización, siempre va antes del useEndpoints
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }


        //Luego de haber creado esta clase y darle cuerpo a sus métodos, procedemos a instanciarla dentro de la clase program.
    }
}
