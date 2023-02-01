namespace UdemyAPICore6.Servicios
{
    //Ejemplo del uso de IHostedService
    // Para inicar el servicio primero debe configurarse en la clase startup
    public class EscribirArchivo : IHostedService
    {
        private readonly IWebHostEnvironment env;
        private readonly string nombreArchivo = "archivo_1.txt" ;
        private Timer timer;

        public EscribirArchivo(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            
            Escribir("Proceso iniciado: " + DateTime.Now.ToString("M-d-yyyy HH:mm:ss"));
            timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Dispose();
            Escribir("PRoceso terminado: " + DateTime.Now.ToString("M-d-yyyy HH:mm:ss"));
            return Task.CompletedTask;
            
        }


        private void Escribir(string mensaje)
        {
            var ruta = $@"{env.ContentRootPath}\wwwroot\{nombreArchivo}";
            using (StreamWriter writer = new StreamWriter(ruta, append: true))
            {
                writer.WriteLine(mensaje);
            }
        }

        public void DoWork(object state)
        {
            Escribir("Proceso en ejecución: "+ DateTime.Now.ToString("M-d-yyyy HH:mm:ss"));

        }
    }
}
