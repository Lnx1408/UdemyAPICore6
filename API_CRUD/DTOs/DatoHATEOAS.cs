namespace API_CRUD.DTOs
{
    public class DatoHATEOAS
    {
        //Esata clase deber permitir solo crearla, pero no modificarla, por eso los set son private
        public string Enlace { get; private set; }
        public string Descripcion { get; private set; }
        public string Metodo { get; private set; }


        public DatoHATEOAS(string enlace, string descripcion, string metodo)
        {
            Enlace = enlace;
            Descripcion = descripcion;
            Metodo = metodo;
        }
        //Luego creamos una clase de recurso
    }
}
