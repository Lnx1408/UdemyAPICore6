using API_CRUD.DTOs;
using API_CRUD.Entidades;
using AutoMapper;

namespace API_CRUD.Utilidades
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AutorDTOC, Autor>();
            CreateMap<Autor, AutorDTOR>();
            CreateMap<Autor, AutorDTORConLibros>()
                .ForMember(autorDTO => autorDTO.Libros, opciones => opciones.MapFrom(MapAutorDTOLibros));


            CreateMap<LibroDTOC, Libro>()
                .ForMember(libro => libro.AutoresLibros, opciones => opciones.MapFrom(MapAutoresLibros));
            CreateMap<Libro, LibroDTOR>();
            CreateMap<Libro, LibroDTORConAutor>()
                .ForMember(libroDTO => libroDTO.Autores, opciones => opciones.MapFrom(MapLibroDTOAutores));


            CreateMap<ComentarioDTOC, Comentario>();
            CreateMap<Comentario, ComentarioDTOR>();




        }

        private List<LibroDTOR> MapAutorDTOLibros(Autor autor, AutorDTOR autorDTOR)
        {
            var resultado = new List<LibroDTOR>();
            if (autor.AutoresLibros == null)
            {
                return resultado;
            }

            foreach (var autorLibro in autor.AutoresLibros)
            {
                resultado.Add(new LibroDTOR()
                {
                    Id = autorLibro.LibroId,
                    Titulo = autorLibro.Libro.Titulo

                });

            }
            return resultado; 
        }

        private List<AutorLibro> MapAutoresLibros(LibroDTOC libroDTOC, Libro libro)
        {
            var resultado = new List<AutorLibro>();
            if(libroDTOC.AutoresIds == null) { return resultado; }

            foreach(var autorId in libroDTOC.AutoresIds)
            {
                resultado.Add(new AutorLibro() { AutorId = autorId });
            }


            return resultado;
        }

        private List<AutorDTOR> MapLibroDTOAutores(Libro libro, LibroDTOR libroDTOR)
        {
            var resultado = new List<AutorDTOR>();
            if(libro.AutoresLibros== null) { return resultado; }

            foreach (var autorLibro in libro.AutoresLibros)
            {
                resultado.Add(new AutorDTOR()
                {
                    Id = autorLibro.AutorId,
                    Nombre = autorLibro.Autor.Nombre
                }); 

            }

            return resultado;
        } 

        
    }
}
