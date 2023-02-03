using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_CRUD.Entidades;
using API_CRUD.Filtros;
using API_CRUD.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API_CRUD.Controllers
{
    [ApiController]
    [Route("api/autores")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AutoresController: ControllerBase
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

        [HttpGet]
        [AllowAnonymous]
        public async Task<List<AutorDTOR>> Get()
        {

            var autores = await context.Autores.ToListAsync();
            return mapper.Map<List<AutorDTOR>>(autores);
        }

        /// <summary>
        /// Se puede colocar nombres a nuestros endpoint para luego retornarlos - (Buenas prácticas)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name ="obtenerAutorID")]
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

            return mapper.Map<AutorDTORConLibros>(autor);
        }



        [HttpGet("{nombre}")]
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
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AutorDTOC autorDTO)
        {
            var nombreDuplicado = await context.Autores.AnyAsync(x => x.Nombre == autorDTO.Nombre);
            if(nombreDuplicado)
            {
                return BadRequest($"Ya existe un autor con el mismo nombre: {autorDTO.Nombre}");
            }

            var autor = mapper.Map<Autor>(autorDTO);


            context.Add(autor);
            await context.SaveChangesAsync();
            
            var autorDTOR = mapper.Map<AutorDTOR>(autor);
            //No se recomienda pasar directamente la entidad, sino un DTO de esta.
            return CreatedAtRoute("obtenerAutorID",new {id = autor.Id},autorDTOR);            

        }



        [HttpPut("{id:int}")]
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

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Autores.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound($"No existe el autor con ID {id}");
            }

            context.Remove(new Autor() { Id = id});
            await context.SaveChangesAsync();
            return Ok();

        }


        [HttpGet("Configuraciones")]
        public ActionResult<string> getConnectionResult()
        {
            string cadena = configuration["connectionStrings:defaultConnection"] + "\n";
            cadena += configuration["apellido"]; 
            return cadena;
        }

    }
}
