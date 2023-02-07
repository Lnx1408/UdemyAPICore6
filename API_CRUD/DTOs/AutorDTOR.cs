namespace API_CRUD.DTOs
{
    /// <summary>
    /// Se extiende de la clase Recursos para poder usar la generacion de enlaces desde el Autorescontroller
    /// </summary>
    public class AutorDTOR: Recurso
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        
    }
}
