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
            server.AddRoute("/favicon.ico", new StaticFilesController().Favico);

            await server.StartAsync(80);
        }
    }
}
