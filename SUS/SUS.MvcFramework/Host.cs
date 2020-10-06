namespace SUS.MvcFramework
{
    using SUS.HTTP;

    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Host
    {
        public static async Task CreateHostAsync(IMvcApplication application, int port = 80)
        {
            var routeTable = new List<Route>();
            application.ConfigureServices();
            application.Configure(routeTable);

            var httpServer = new HttpServer(routeTable);

            await httpServer.StartAsync(port);
        }
    }
}
