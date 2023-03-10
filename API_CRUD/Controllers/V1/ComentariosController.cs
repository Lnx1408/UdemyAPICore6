using API_CRUD.DTOs;
using API_CRUD.Entidades;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_CRUD.Controllers.V1
{
    [ApiController]
    [Route("api/libros/{libroId:int}/comentarios")]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public ComentariosController(AppDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }

        [HttpPost(Name = "registrarComentario")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Post(int libroId, ComentarioDTOC comentarioDTOC)
        {

            //Es recomendable obtener los datos del usuario por medio de los claims
            var emailClaim = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var email = emailClaim.Value;
            var usuario = await userManager.FindByEmailAsync(email);
            var usuarioId = usuario.Id;
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id.Equals(libroId));

            if (!existeLibro)
            {
                return NotFound();
            }

            var comentario = mapper.Map<Comentario>(comentarioDTOC);
            comentario.LibroId = libroId;
            comentario.UsuarioId = usuarioId;
            context.Add(comentario);
            await context.SaveChangesAsync();
            var comentarioDTOR = mapper.Map<ComentarioDTOR>(comentario);
            return CreatedAtRoute("ObtenerComentarioId", new { id = comentario.Id, libroId = comentario.LibroId }, comentarioDTOR);

        }

        [HttpPut("{id:int}", Name = "actualizarComentario")]
        public async Task<ActionResult> Put(int libroId, int id, ComentarioDTOC comentarioDTOC)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id.Equals(libroId));
            if (!existeLibro)
            {
                return NotFound($"No existe el Libro {libroId}");
            }

            var existeComentario = await context.Comentarios.AnyAsync(comentarioDTOC => comentarioDTOC.Id.Equals(id));
            if (!existeComentario)
            {
                return NotFound($"No existe el Comentario {id}");
            }
            var comentario = mapper.Map<Comentario>(comentarioDTOC);
            comentario.Id = id;
            comentario.LibroId = libroId;

            context.Update(comentario);
            await context.SaveChangesAsync();

            return NoContent();

        }


        [HttpGet("id/{id:int}", Name = "obtenerComentarioId")]
        public async Task<ActionResult<ComentarioDTOR>> GetById(int id)
        {
            var comentario = await context.Comentarios.FirstOrDefaultAsync(comentarioDB => comentarioDB.Id.Equals(id));
            if (comentario == null)
            {
                return NotFound();
            }

            return mapper.Map<ComentarioDTOR>(comentario);
        }


        [HttpGet(Name = "obtenerComentarioLibro")]
        public async Task<ActionResult<List<ComentarioDTOR>>> Get(int libroId)
        {
            var existeLibro = await context.Libros.AnyAsync(libroDB => libroDB.Id == libroId);
            if (!existeLibro)
            {
                return NotFound();
            }

            var comentarios = await context.Comentarios.Where(comentarioDB => comentarioDB.LibroId.Equals(libroId)).ToListAsync();

            return mapper.Map<List<ComentarioDTOR>>(comentarios);
        }

    }
}
