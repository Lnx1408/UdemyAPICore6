using API_CRUD.DTOs;
using API_CRUD.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CRUD.Controllers
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController: ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public ComentariosController(AppDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpPost]
        public async Task<ActionResult> Post(int libroId, ComentarioDTOC comentarioDTOC)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB=> libroDB.Id.Equals(libroId));
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioDTOC);
            comentario.LibroId = libroId;

            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTOR = mapper.Map<ComentarioDTOR>(comentario);
            return CreatedAtRoute("ObtenerComentarioId", new {id = comentario.Id, libroId = comentario.LibroId}, comentarioDTOR);

        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioDTOC comentarioDTOC)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id.Equals(libroId));
            if (!existeLibro)
            {
                return NotFound($"No existe el Libro {libroId}");
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDTOC => comentarioDTOC.Id.Equals(id));
            if(!existeComentario)
            {
                return NotFound($"No existe el Comentario {id}");
            }
            var comentario = mapper.Map<Comentario>(comentarioDTOC);
            comentario.Id= id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();

        }


        [HttpGet("{id:int}", Name ="ObtenerComentarioId")]
        public async Task<ActionResult<ComentarioDTOR>> GetById(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id.Equals(id));
            if(comentario==null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTOR>(comentario);
        }

        [HttpGet]
        public async Task<ActionResult<List<ComentarioDTOR>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id.Equals(libroId));
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(comentarioDB => comentarioDB.LibroId.Equals(libroId)).ToListAsync();

            return mapper.Map<List<ComentarioDTOR>>(comentarios);
        }
        
    }
}
