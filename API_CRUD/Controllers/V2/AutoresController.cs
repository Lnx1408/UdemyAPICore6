using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_CRUD.Entidades;
using API_CRUD.Filtros;
using API_CRUD.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using API_CRUD.Utilidades;

namespace API_CRUD.Controllers.V2
{
    [ApiController]
    [Route("api/autores")]
    [CabeceraPresente("x-version", "2")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")] //La política se establece en el servicio AddAuthorization de Startup
    public class AutoresController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IConfiguration configuration;

        public AutoresController(AppDbContext context, IMapper mapper, IConfiguration configuration)
        {
            this.context = context;
            this.mapper = mapper;
            this.configuration = configuration;
        }

        [HttpGet(Name = "obtenerAutores")]
        [AllowAnonymous]
        public async Task<List<AutorDTOR>> Get()
        {

            var autores = await context.Autores.ToListAsync();
            autores.ForEach(autor => autor.Nombre = autor.Nombre.ToUpper());
            return mapper.Map<List<AutorDTOR>>(autores);
        }

        /// <summary>
        /// Se puede colocar nombres a nuestros endpoint para luego retornarlos - (Buenas prácticas)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "obtenerAutorID")]
        public async Task<ActionResult<AutorDTORConLibros>> buscarAutorId(int id)
        {
            var autor = await context.Autores
                .Include(autorDb => autorDb.AutoresLibros)
                .ThenInclude(autorLibroDb => autorLibroDb.Libro)
                .FirstOrDefaultAsync(autorDb => autorDb.Id.Equals(id));

            if (autor == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<AutorDTORConLibros>(autor);

            generarEnlaces(dto);

            return dto;
        }



        [HttpGet("{nombre}", Name = "obtenerAutorNombre")]
        public async Task<ActionResult<List<AutorDTOR>>> buscarAutorNombre([FromRoute] string nombre)
        {
            var autores = await context.Autores.Where(autorDB => autorDB.Nombre.Contains(nombre)).ToListAsync();
            if (autores == null)
            {
                return NotFound();
            }
            return mapper.Map<List<AutorDTOR>>(autores);
        }

        /// <summary>
        /// En los mètodos de envío es recomendable retornar el objeto creado en lugar de un Ok();
        /// </summary>
        /// <param name="autorDTO"></param>
        /// <returns></returns>
        [HttpPost(Name = "registrarAutor")]
        public async Task<ActionResult> Post([FromBody] AutorDTOC autorDTO)
        {
            var nombreDuplicado = await context.Autores.AnyAsync(x => x.Nombre == autorDTO.Nombre);
            if (nombreDuplicado)
            {
                return BadRequest($"Ya existe un autor con el mismo nombre: {autorDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorDTO);


            context.Add(autor);
            await context.SaveChangesAsync();

            var autorDTOR = mapper.Map<AutorDTOR>(autor);
            //No se recomienda pasar directamente la entidad, sino un DTO de esta.
            return CreatedAtRoute("obtenerAutorID", new { id = autor.Id }, autorDTOR);

        }



        [HttpPut("{id:int}", Name = "actualizarAutor")]
        public async Task<ActionResult> Put(AutorDTOC autorDTO, int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound();
            }


            var autor = mapper.Map<Autor>(autorDTO);
            autor.Id = id;


            context.Update(autor);
            await context.SaveChangesAsync();
            return NoContent();

        }

        [HttpDelete("{id:int}", Name = "eliminarAutor")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound($"No existe el autor con ID {id}");
            }

            context.Remove(new Autor() { Id = id });
            await context.SaveChangesAsync();
            return Ok();

        }


        [HttpGet("Configuraciones", Name = "obtenerConfiguraciones")]
        public ActionResult<string> getConnectionResult()
        {
            string cadena = configuration["connectionStrings:defaultConnection"] + "\n";
            cadena += configuration["apellido"];
            return cadena;
        }

        /// <summary>
        /// Primera forma para generar los enlaces desde el controller, la 2da está en LibrosController.
        /// </summary>
        /// <param name="autorDTOR"></param>
        private void generarEnlaces(AutorDTOR autorDTOR)
        {
            autorDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutorID", new { id = autorDTOR.Id }), descripcion: "self", metodo: "GET"));

            autorDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("actualizarAutor", new { id = autorDTOR.Id }), descripcion: "autor-id", metodo: "PUT"));

            autorDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("eliminarAutor", new { id = autorDTOR.Id }), descripcion: "autor-id", metodo: "DELETE"));



        }

    }
}
