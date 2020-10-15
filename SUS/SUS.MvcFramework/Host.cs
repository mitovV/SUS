namespace SUS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    using HTTP;

    public class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80)
        {
            var routeTable = new List<Route>();
            IServiceCollection serviceCollection = new ServiceCollection();

            application.ConfigureServices(serviceCollection);
            application.Configure(routeTable);

            AutoRegisterStaticFiles(routeTable);
            AutoRegisterRoutes(routeTable, application, serviceCollection);

            var httpServer = new HttpServer(routeTable);

            await httpServer.StartAsync(port);
        }

        private static void AutoRegisterRoutes(List<Route> routeTable, IMvcApplication application, IServiceCollection serviceCollection)
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


                    var route = new Route(url, httpMethod, (request) => ExecuteAction(request, serviceCollection, controllerType, method));

                    routeTable.Add(route);
                }
            }
        }

        private static HttpResponse ExecuteAction(HttpRequest request, IServiceCollection serviceCollection, Type controllerType, MethodInfo method)
        {
            var instance = serviceCollection.CreateInstance(controllerType) as Controller;
            instance.HttpRequest = request;

            var arguments = new List<object>();

            var parameters = method.GetParameters();

            foreach (var parameter in parameters)
            {
                var httpParameterValue = GetParameterFromRequest(request, parameter.Name);
                var parameterValue = Convert.ChangeType(httpParameterValue, parameter.ParameterType);

                if (parameterValue == null && parameter.ParameterType != typeof(string))
                {
                    parameterValue = Activator.CreateInstance(parameter.ParameterType);

                    var properties = parameter.ParameterType.GetProperties();

                    foreach (var property in properties)
                    {
                        httpParameterValue = GetParameterFromRequest(request, property.Name);
                        var propertyParameterValue = Convert.ChangeType(httpParameterValue, property.PropertyType);
                        property.SetValue(parameterValue, propertyParameterValue);
                    }
                }

                arguments.Add(parameterValue);
            }

            var response = method.Invoke(instance, arguments.ToArray()) as HttpResponse;

            return response;
        }

        private static string GetParameterFromRequest(HttpRequest request, string parameterName)
        {
            if (request.FormData.Any(x => x.Key.ToLower() == parameterName.ToLower()))
            {
                return request.FormData.FirstOrDefault(x => x.Key.ToLower() == parameterName.ToLower()).Value;
            }

            if (request.QueryData.Any(x => x.Key.ToLower() == parameterName.ToLower()))
            {
                return request.QueryData.FirstOrDefault(x => x.Key.ToLower() == parameterName.ToLower()).Value;
            }

            return null;
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
