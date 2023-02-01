﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_CRUD.Entidades;
using API_CRUD.DTOs;
using AutoMapper;

namespace API_CRUD.Controllers
{
    [ApiController]
    [Route("api/libros")]
    public class LibrosController: ControllerBase
    {
        private readonly AppDbContext context;
        private readonly IMapper mapper;

        public LibrosController(AppDbContext context, IMapper mapper) { 
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet("{id:int}", Name ="ObtenerLibrosID")]
        public async Task<ActionResult<LibroDTORConAutor>> Get(int id)
        {
            //* Traer los comentarios cuando se cargue los libros
            //var libro = await context.Libros.Include(libroDB=> libroDB.Comentarios).FirstOrDefaultAsync(libroDB => libroDB.Id.Equals(id));

            //Estraer los datos de autorLibro, y a su vez por medio del ThenInclude acceder a la entidad de libro y sus atributos.
            var libro = await context.Libros
                .Include(libroDb=> libroDb.AutoresLibros)
                .ThenInclude(autorLibroDb => autorLibroDb.Autor)
                .FirstOrDefaultAsync(libroDB => libroDB.Id.Equals(id));

            libro.AutoresLibros = libro.AutoresLibros.OrderBy(x=> x.Orden).ToList();

            return mapper.Map<LibroDTORConAutor>(libro);
        }

        [HttpPost]
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
        [HttpPut("{id:int}")]
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
    }
}
