namespace API_CRUD.DTOs
{
    /// <summary>
    /// Clases creadas para solucionar el problema de datos nulos que se creaban por los ciclos infinitos.
    /// </summary>
    public class AutorDTORConLibros: AutorDTOR
    {
        public List<LibroDTOR> Libros { get; set; }
    }
}
