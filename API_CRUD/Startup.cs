using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using API_CRUD.Filtros;
using API_CRUD.Middlewares;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using API_CRUD.Servicios;
using API_CRUD.Utilidades;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API_CRUD
{
    public class Startup
    {
        public Startup(IConfiguration configuration) {

            //Con esta línea de comando hace que los claims no sean mapeados con nombres extraños, sino con los que realmente hemos definido.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
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
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                //Configurar el bearer
                .AddJwtBearer(opciones=> opciones.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    //No se valida la audiencia
                    ValidateAudience= false,
                    //Validar el tiempo de vida del token
                    ValidateLifetime= true,
                    //Validar la firma
                    ValidateIssuerSigningKey= true,
                    //Configurar la firma
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llaveJwt"])),
                    ClockSkew = TimeSpan.Zero

                });

            services.AddControllers(opciones =>
            {
                //Añadir el filtro de manera global
                opciones.Filters.Add(typeof(FiltroExepcion));

                //Controlador para agrupar por versiones
                opciones.Conventions.Add(new SwaggerVersiones());

            }).AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles).AddNewtonsoftJson();

            services.AddDbContext<AppDbContext>(options=> options.UseSqlServer(Configuration.GetConnectionString("defaultConnection")));
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();

            //Añadimos las configuraciones para poder introducir el token en nuestros endpoints y se muestre qué endpoint necesitan autorización y la capacidad de ingresar el tiken para autorizar el uso de las APIS
            services.AddSwaggerGen(c =>
            {

                //Para aplicar versionamiento de API
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1"});
                c.SwaggerDoc("v2", new OpenApiInfo { Title = "WebApi", Version = "v2" });
                c.OperationFilter<AgregarParametroVersion>();

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[]{}
                    }
                });
            });

            services.AddAutoMapper(typeof(Startup));


            //Servicio de Identity
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();

            //Autorización basada en claims
            services.AddAuthorization(opciones =>
            {
                //Se añaden las políticas necesarias para el acceso
                opciones.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin"));
                opciones.AddPolicy("EsVendedor", politica => politica.RequireClaim("esVendedor"));
            });


            //Permite la encriptación y decriptación de datos - Servicio de protección de datos
            services.AddDataProtection();
            services.AddTransient<HashService>();

            //Configuración del servicio CORS
            services.AddCors(opciones =>
            {
                opciones.AddDefaultPolicy(builder =>
                {
                    //Se puede colocar la URL externa donde podrán acceder a nuestra Web API
                    builder.WithOrigins("https://apirequest.io").AllowAnyMethod().AllowAnyHeader();
                    // Se coloca solo si es necesario exponer las cabeceras
                    //.WithExposedHeaders();
                });
            });


            services.AddTransient<GeneradorEnlaces>();
            services.AddTransient<HATEOASLibroFilterAttribute>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();


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
                app.UseSwaggerUI(
                    c=>
                    {
                        //Configuarión del endpoint de versiones.
                        c.SwaggerEndpoint("swagger/v1/swagger.json", "WebAPI v1");
                        c.SwaggerEndpoint("swagger/v2/swagger.json", "WebAPI v2");
                    }
                    );
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //Añadir el uso de cors
            app.UseCors();
            //Permite usar el filtro de autorización, siempre va antes del useEndpoints
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
