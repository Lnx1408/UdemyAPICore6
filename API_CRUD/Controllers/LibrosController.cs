using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_CRUD.Entidades;
using API_CRUD.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace API_CRUD.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    [ApiController]
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly IAuthorizationService authorizationService;

        public LibrosController(AppDbContext context, IMapper mapper, IAuthorizationService authorizationService) { 
            this.context = context;
            this.mapper = mapper;
            this.authorizationService = authorizationService;
        }

        [HttpGet(Name = "obtenerLibros")]
        [AllowAnonymous]
        public async Task<ColeccionRecursos<LibroDTOR>> Get()
        {

            var libros = await context.Libros.ToListAsync();
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");


            var dtos = mapper.Map<List<LibroDTOR>>(libros);
            dtos.ForEach(dto => generarEnlaces(dto, esAdmin.Succeeded));

            var enlacesExtras = new ColeccionRecursos<LibroDTOR> { Valores = dtos };

            enlacesExtras.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("obtenerLibros", new { }), 
                descripcion: "self", 
                metodo: "GET"));
            if(esAdmin.Succeeded)
            {
                enlacesExtras.Enlaces.Add(new DatoHATEOAS(
                enlace: Url.Link("registrarLibro", new { }),
                descripcion: "libro-registrar",
                metodo: "POST"));
            }
            


            return enlacesExtras;
        }

        [AllowAnonymous]
        [HttpGet("{id:int}", Name ="obtenerLibrosID")]
        public async Task<ActionResult<LibroDTORConAutor>> Get(int id)
        {
            //* Traer los comentarios cuando se cargue los libros
            //var libro = await context.Libros.Include(libroDB=> libroDB.Comentarios).FirstOrDefaultAsync(libroDB => libroDB.Id.Equals(id));

            //Estraer los datos de autorLibro, y a su vez por medio del ThenInclude acceder a la entidad de libro y sus atributos.
            var libro = await context.Libros
                .Include(libroDb=> libroDb.AutoresLibros)
                .ThenInclude(autorLibroDb => autorLibroDb.Autor)
                .FirstOrDefaultAsync(libroDB => libroDB.Id.Equals(id));

            if (libro == null)
            {
                return NotFound();
            }

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x=> x.Orden).ToList();

            var dto = mapper.Map<LibroDTORConAutor>(libro);
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");
            generarEnlaces(dto, esAdmin.Succeeded);
            return dto;

        }

        [HttpPost(Name ="registrarLibro")]
        public async Task<ActionResult> Post(LibroDTOC libroDTOC)
        {
            if (libroDTOC.AutoresIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }


            var autoresIds = await context.Autores.Where(autorDb => libroDTOC.AutoresIds.Contains(autorDb.Id)).Select(x=> x.Id).ToListAsync();
            if (libroDTOC.AutoresIds.Count != autoresIds.Count)
            {
                return BadRequest("No existe uno de los autores enviados");
            }

            var libro = mapper.Map<Libro>(libroDTOC);

            asignarOrdenAutores(libro);

            context.Add(libro);
            await context.SaveChangesAsync();
            var LibroDTOR = mapper.Map<LibroDTOR>(libro);
            return CreatedAtRoute("ObtenerLibrosID", new {id = libro.Id}, LibroDTOR);
        }

        /// <summary>
        /// Actualizar un libro y sus dependencias (Autores)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="libroDTOC"></param>
        /// <returns></returns>
        [HttpPut("{id:int}", Name = "actualizarLibro")]
        public async Task<ActionResult> Put(int id, LibroDTOC libroDTOC)
        {
             var libroDb = await context.Libros
                .Include(autoresLibrosDb=>autoresLibrosDb.AutoresLibros)
                .FirstOrDefaultAsync(LibrosDb => LibrosDb.Id.Equals(id));

            if (libroDb == null)
            {
                return NotFound();
            }

            libroDb = mapper.Map(libroDTOC, libroDb);
            asignarOrdenAutores(libroDb);

            await context.SaveChangesAsync();
            return NoContent();
        }

        private void asignarOrdenAutores(Libro libro)
        {
            if (libro.AutoresLibros != null)
            {
                for (int i = 0; i < libro.AutoresLibros.Count; i++)
                {
                    libro.AutoresLibros[i].Orden = i;
                }
            }
        }
        [HttpPatch(Name ="actualizarParcialmenteLibro")]
        public async Task<ActionResult> Patch(int id, JsonPatchDocument<LibroPatchDTO> patchDocument)
        {
            if (patchDocument == null)
            {
                return BadRequest();
            }

            var libroDb = await context.Libros.FirstOrDefaultAsync(x=>x.Id == id);
            if(libroDb == null)
            {
                return NotFound();
            }

            var libroDTO = mapper.Map<LibroPatchDTO>(libroDb);

            patchDocument.ApplyTo(libroDTO, ModelState);

            var esValido = TryValidateModel(libroDTO);

            if(!esValido)
            {
                return BadRequest(ModelState);
            }

            mapper.Map(libroDTO, libroDb);

            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id:int}", Name ="eliminarLibro")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libros.AnyAsync(x => x.Id.Equals(id));
            if (!existe)
            {
                return NotFound($"No existe el libro con ID {id}");
            }

            context.Remove(new Libro() { Id = id });
            await context.SaveChangesAsync();
            return Ok();

        }


        private void generarEnlaces(LibroDTOR libroDTOR, bool esAdmin)
        {
            libroDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerLibrosID", new { id = libroDTOR.Id }), descripcion: "self", metodo: "GET"));

            if(esAdmin)
            {
                libroDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("actualizarLibro", new { id = libroDTOR.Id }), descripcion: "autor-actualizar", metodo: "PUT"));

                libroDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("eliminarLibro", new { id = libroDTOR.Id }), descripcion: "autor-eliminar", metodo: "DELETE"));

            }

        }
    }

    
    
}
