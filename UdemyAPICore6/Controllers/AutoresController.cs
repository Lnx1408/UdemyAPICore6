using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UdemyAPICore6.Entidades;
using UdemyAPICore6.Filtros;
using UdemyAPICore6.Servicios;

namespace UdemyAPICore6.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController: ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IServicio servicio;
        private readonly ServicioScope servicioScope;
        private readonly ServicioSingleton servicioSingleton;
        private readonly ServicioTransient servicioTransient;
        private readonly ILogger<AutoresController> logger;

        public AutoresController(AppDbContext context, IServicio servicio, ServicioScope servicioScope, ServicioSingleton servicioSingleton, ServicioTransient servicioTransient, ILogger<AutoresController> logger)
        {
            this.context = context;
            this.servicio = servicio;
            this.servicioScope = servicioScope;
            this.servicioSingleton = servicioSingleton;
            this.servicioTransient = servicioTransient;
            this.logger = logger;
        }

        /// <summary>
        /// Mostrar ejemplo del ciclo de vida de un servicio
        /// </summary>
        /// <returns></returns>
        [HttpGet("GUID")]
        //Haciendo uso de este filtro "ResponseCache" nos permite guardar esta información durante el tiempo que especifiquemos antes de que se vuelva a actualizar.
        [ResponseCache(Duration = 10)]
        public ActionResult obtenerGuid()
        {
            return Ok( new{
                AutoresController_Transient = servicioTransient.Guid,
                servicioA_Transient = servicio.obtenerTransient(),
                
                AutoresController_Scope = servicioScope.Guid,
                servicioA_Scope = servicio.obtenerScope(),

                AutoresController_Singleton = servicioSingleton.Guid,
                servicioA_Singleton = servicio.obtenerSingleton()
            });
        }

        /// <summary>
        /// REGLAS DE RUTAS
        /// </summary>
        /// <returns></returns>
        [HttpGet] // Ruta por defecto, corresponde a api/autores
        [HttpGet("listado")] //Ruta alternativa, responde a api/autores/listado
        [HttpGet("/listado")] //Ruta alternativa fuera de jerarquia, responde a /listado
        //[Authorize]
        //Usar el filtro personalizado
        [ServiceFilter(typeof(FiltroAccionCustom))]
        public async Task<List<Autor>> Get()
        {
            //throw new NotImplementedException();
            logger.LogInformation("Estamos obteniendo los autores");
            logger.LogWarning("Prueba de warning");
            servicio.RealizarTarea();
            return await context.Autores.Include(x=> x.Libros).ToListAsync();  
        
        }
        [HttpGet("primero")]
        public async Task<ActionResult<Autor>> primerAutor()
        {
            return await context.Autores.Include(x => x.Libros).FirstOrDefaultAsync();

        }
        //Permitir varios parametros con uno opcional
        [HttpGet("{id:int}/{param2?}")]
        public async Task<ActionResult<Autor>> buscarAutor(int id)
        {
            var existe = await context.Autores.AnyAsync(x=> x.Id.Equals(id));
            if (!existe)
            {
                return BadRequest($"No existe el Autor de id: {id}");
            }
            return await context.Autores.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpGet("segundo/{id:int}/{param = intellij}/{y=XDXD}")]
        public ActionResult<Autor> buscarAutor2(int id, string param, string y)
        {
            var autor = context.Autores.FirstOrDefault(x => x.Id.Equals(id));
            if (autor == null)
            {
                return NotFound();
            }
            return autor;
        }


        [HttpGet("{nombre}")]
        public async Task<ActionResult<Autor>> buscarAutorNombre([FromRoute] string nombre)
        {
            var autor = await context.Autores.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Nombre.Contains(nombre));
            if (autor == null)
            {
                return NotFound();
            }
            return autor; 
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Autor autor)
        {
            var nombreDuplicado = await context.Autores.AnyAsync(x => x.Nombre == autor.Nombre);
            if(nombreDuplicado)
            {
                return BadRequest($"Ya existe un autor con el mismo nombre: {autor.Nombre}");
            }
            context.Add(autor);
            await context.SaveChangesAsync();
            return Ok();            

        }



        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(Autor autor, int id)
        {
            if(autor.Id != id)
            {
                return BadRequest("El Id no es correcto");
            }

            var existe = await context.Autores.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound();
            }

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound();
            }

            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return Ok();

        }

    }
}
