namespace MyFirstMvcApp
{
    using System;
    using System.Threading.Tasks;

    using SUS.HTTP;

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new HttpServer();

            server.AddRoute("/", HomePage);

            await server.StartAsync(80);
        }

        private static HttpRespose HomePage(HttpRequest arg)
        {
            throw new NotImplementedException();
        }
    }
}
