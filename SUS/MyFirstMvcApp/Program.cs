namespace MyFirstMvcApp
{
    using System;
    using System.IO;
    using System.Text;
    using System.Threading.Tasks;

    using SUS.HTTP;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new HttpServer();

            server.AddRoute("/", HomePage);
            server.AddRoute("/favicon.ico", Favicon);

            await server.StartAsync(80);
        }

        private static HttpResponse Favicon(HttpRequest request)
        {
            var responseBytes = File.ReadAllBytes("wwwroot/favicon.ico");

            var respose = new HttpResponse("image/x-icon", responseBytes);

            return respose;
        }

        private static HttpResponse HomePage(HttpRequest request)
        {
            var responseHtml = "<h1>Welcome!</h1>";
            var responseBytes = Encoding.UTF8.GetBytes(responseHtml);

            var response = new HttpResponse("text/html",responseBytes);

            return response;
        }
    }
}
