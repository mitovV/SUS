namespace MyFirstMvcApp
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using MyFirstMvcApp.Controllers;

    using SUS.HTTP;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new HttpServer();

            server.AddRoute("/", new HomeController().Index);
            server.AddRoute("/users/login", new UsersController().Login);
            server.AddRoute("/users/register", new UsersController().Register);

            server.AddRoute("/favicon.ico", new StaticFilesController().Favico);
            server.AddRoute("/wwwroot/css/site.css", new StaticFilesController().SiteCss);

            await server.StartAsync(80);
        }
    }
}
