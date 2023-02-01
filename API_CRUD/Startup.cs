using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using API_CRUD.Filtros;
using API_CRUD.Middlewares;

namespace API_CRUD
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
            services.AddAutoMapper(typeof(Startup));


        }


        /// <summary>
        /// Este método lo complementamos con instrucciones de la clase program.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {

            //2da Forma: Se accede directamente al método de la clase estática. 
            app.UseLoguearRespuestaHTTP();
            
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Permite usar el filtro de autorización, siempre va antes del useEndpoints
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
