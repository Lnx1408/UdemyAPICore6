using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace API_CRUD.Utilidades
{
    public class SwaggerVersiones : IControllerModelConvention
    {
        /// <summary>
        /// Convención que agrupa el API por versiones.
        /// </summary>
        /// <param name="controller"></param>
        public void Apply(ControllerModel controller)
        {
            var namespaceControlador = controller.ControllerType.Namespace;
            var versionAPI = namespaceControlador.Split(".").Last().ToLower();
            controller.ApiExplorer.GroupName = versionAPI;

        }
    }
}
