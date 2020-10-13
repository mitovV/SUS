namespace SUS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using HTTP;

    public class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80)
        {
            var routeTable = new List<Route>();

            AutoRegisterStaticFiles(routeTable);
            AutoRegisterRoutes(routeTable, application);

            application.ConfigureServices();
            application.Configure(routeTable);

            var httpServer = new HttpServer(routeTable);

            await httpServer.StartAsync(port);
        }

        private static void AutoRegisterRoutes(List<Route> routeTable, IMvcApplication application)
        {
            var controllerTypes = application
                .GetType()
                .Assembly
                .GetTypes()
                .Where(x => x.IsClass && x.IsPublic && !x.IsAbstract && x.IsSubclassOf(typeof(Controller)))
                .ToArray();

            foreach (var controllerType in controllerTypes)
            {
                var methods = controllerType
                    .GetMethods()
                    .Where(x => x.IsPublic && !x.IsStatic && !x.IsConstructor && !x.IsSpecialName && x.DeclaringType == controllerType)
                    .ToArray();

                foreach (var method in methods)
                {
                    var url = "/" + controllerType.Name.Replace("Controller", string.Empty) + "/" + method.Name;


                    var attribute = method
                        .GetCustomAttributes(false)
                        .Where(x => x.GetType().IsSubclassOf(typeof(BaseHttpAttribute)))
                        .FirstOrDefault() as BaseHttpAttribute;

                    var httpMethod = HttpMethod.GET;

                    if (!string.IsNullOrEmpty(attribute?.Url))
                    {
                        url = attribute.Url;
                    }

                    if (attribute != null)
                    {
                        httpMethod = attribute.HttpMethod;
                    }


                    var route = new Route(url, httpMethod, (reques) =>
                     {
                         var instance = Activator.CreateInstance(controllerType);
                         var response = method.Invoke(instance, new object[] { reques }) as HttpResponse;

                         return response;
                     });

                    routeTable.Add(route);
                }
            }
        }

        private static void AutoRegisterStaticFiles(List<Route> routeTable)
        {
            var staticFiles = Directory.GetFiles("wwwroot", "*", SearchOption.AllDirectories);

            foreach (var file in staticFiles)
            {
                var url = file.Replace("wwwroot", string.Empty).Replace("\\", "/");

                var route = new Route(url, HttpMethod.GET, (request) =>
                {
                    var content = File.ReadAllBytes(file);

                    var fileExt = new FileInfo(file).Extension;

                    var contentType = fileExt switch
                    {
                        ".html" => "text/html",
                        ".txt" => "text/plain",
                        ".ico" => "image/x-icon",
                        ".js" => "text/javascript",
                        ".png" => "image/png",
                        ".jpg" => "image/jpg",
                        ".jpeg" => "image/jpg",
                        ".gif" => "image/gif",
                        ".css" => "text/css",
                        _ => "text/plain"
                    };

                    return new HttpResponse(contentType, content);
                });

                routeTable.Add(route);
            }
        }
    }
}
