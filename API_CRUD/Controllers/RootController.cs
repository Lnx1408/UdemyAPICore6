using API_CRUD.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API_CRUD.Controllers
{
    [ApiController]
    [Route("api")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RootController:ControllerBase
    {
        private readonly IAuthorizationService authorizationService;

        public RootController(IAuthorizationService authorizationService)
        {
            this.authorizationService = authorizationService;
        }
        [HttpGet(Name = "ObtenerRoot")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DatoHATEOAS>>> Get()
        {
            var datosHateoas = new List<DatoHATEOAS>();

            //Obtenemos el nivel de acceso que tiene el usuario, en este caso verificamos que sea Administrador
            var esAdmin = await authorizationService.AuthorizeAsync(User, "esAdmin");


            //Los usuarios por defecto solo tendrán la visualización de los endpoints a los que tengan acceso (Los que no necesitan autenticación).
            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerRoot", new { }), descripcion: "self", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo: "POST"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("encriptar", new { }), descripcion: "encriptar", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("encriptarConHash", new { }), descripcion: "encriptar-hash", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("registrar", new { }), descripcion: "registrar-usuario", metodo: "POST"));
            
            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("login", new { }), descripcion: "login", metodo: "GET"));

            datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutores", new { }), descripcion: "autores", metodo: "GET"));

            

            //Solo si el usuario registrado es administrador se le mostrará la siguiente información.
            if (esAdmin.Succeeded)
            {
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("RegistrarAutor", new { }), descripcion: "autor-crear", metodo: "POST"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("obtenerAutorID", new { }), descripcion: "autor-id", metodo: "GET"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ActualizarAutor", new { }), descripcion: "autor-actualizar", metodo: "PUT"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerAutorNombre", new { }), descripcion: "autor-nombre", metodo: "GET"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("EliminarAutor", new { }), descripcion: "autor-eliminar", metodo: "DELETE"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerConfiguraciones", new { }), descripcion: "configuraciones", metodo: "GET"));





                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("RegistrarLibro", new { }), descripcion: "libro-crear", metodo: "GET"));
                datosHateoas.Add(new DatoHATEOAS(enlace: Url.Link("ObtenerLibrosID", new { }), descripcion: "libro-id", metodo: "POST"));

                    

            }
            return datosHateoas;

        }
    }
}
