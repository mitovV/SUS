namespace SUS.HTTP
{
    using System;
    using System.Threading.Tasks;

    public interface IHttpServer
    {
        void AddRoute(string path, Func<HttpRequest, HttpRespose> action);

        Task StartAsync(int port);
    }
}
