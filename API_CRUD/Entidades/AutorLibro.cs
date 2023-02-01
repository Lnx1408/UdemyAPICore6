namespace API_CRUD.Entidades
{
    /// <summary>
    /// Clase para establecer la relación de muchos a muchos
    /// </summary>
    public class AutorLibro
    {
        public int LibroId { get; set; }
        public int AutorId { get; set; }
        public int Orden { get; set; }


        //Propiedades de navegación
        public Libro Libro { get; set; }
        public Autor Autor { get; set; }
    }
}
