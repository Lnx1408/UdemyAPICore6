using API_CRUD.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using System;

namespace API_CRUD.Servicios
{
    public class GeneradorEnlaces
    {
        private readonly IAuthorizationService authorizationService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IActionContextAccessor actionContextAccessor;

        public GeneradorEnlaces(IAuthorizationService authorizationService, IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor)
        {
            this.authorizationService = authorizationService;
            this.httpContextAccessor = httpContextAccessor;
            this.actionContextAccessor = actionContextAccessor;
        }

        private IUrlHelper ConstruirURLHelper()
        {
            var factoria = httpContextAccessor.HttpContext.RequestServices.GetRequiredService<IUrlHelperFactory>();
            return factoria.GetUrlHelper(actionContextAccessor.ActionContext);

        }


        private async Task<bool> EsAdmin()
        {
            var httpContext = httpContextAccessor.HttpContext;
            var resultado = await authorizationService.AuthorizeAsync(httpContext.User, "esAdmin");
            return resultado.Succeeded;
        }
        public async Task GenerarEnlaces(LibroDTOR libroDTOR)
        {

            var esAdmin = await EsAdmin();

            var Url = ConstruirURLHelper();

            libroDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("obtenerLibrosID", new { id = libroDTOR.Id }), descripcion: "self", metodo: "GET"));

            if (esAdmin)
            {
                libroDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("actualizarLibro", new { id = libroDTOR.Id }), descripcion: "autor-actualizar", metodo: "PUT"));

                libroDTOR.Enlaces.Add(new DatoHATEOAS(enlace: Url.Link("eliminarLibro", new { id = libroDTOR.Id }), descripcion: "autor-eliminar", metodo: "DELETE"));

            }

            
        }
    }
}
