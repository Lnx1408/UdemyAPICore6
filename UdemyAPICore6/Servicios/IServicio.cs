namespace UdemyAPICore6.Servicios
{
    public interface IServicio
    {
        Guid obtenerScope();
        Guid obtenerSingleton();
        Guid obtenerTransient();
        void RealizarTarea();
    }

    public class ServicioA: IServicio
    {
        private readonly ILogger<ServicioA> logger;
        private readonly ServicioTransient servicioTransient;
        private readonly ServicioScope servicioScope;
        private readonly ServicioSingleton servicioSingleton;

        public ServicioA(ILogger<ServicioA> logger, ServicioTransient servicioTransient, ServicioScope servicioScope, ServicioSingleton servicioSingleton)
        {
            this.logger = logger;
            this.servicioTransient = servicioTransient;
            this.servicioScope = servicioScope;
            this.servicioSingleton = servicioSingleton;
        }

        public Guid obtenerTransient() { return servicioTransient.Guid; }
        public Guid obtenerScope() { return servicioScope.Guid; }
        public Guid obtenerSingleton() { return servicioSingleton.Guid; }


        public void RealizarTarea()
        {
            
        }
    }

    public class ServicioB : IServicio
    {
        public Guid obtenerScope()
        {
            throw new NotImplementedException();
        }

        public Guid obtenerSingleton()
        {
            throw new NotImplementedException();
        }

        public Guid obtenerTransient()
        {
            throw new NotImplementedException();
        }

        public void RealizarTarea()
        {
            throw new NotImplementedException();
        }
    }


    public class ServicioTransient
    {
        //Crea una cadena de caracteres aleatoria
        public Guid Guid = Guid.NewGuid();
    }


    public class ServicioScope
    {
        //Crea una cadena de caracteres aleatoria
        public Guid Guid = Guid.NewGuid();
    }

    public class ServicioSingleton
    {
        //Crea una cadena de caracteres aleatoria
        public Guid Guid = Guid.NewGuid();
    }

}
