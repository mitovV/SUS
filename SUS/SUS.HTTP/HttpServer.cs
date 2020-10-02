namespace SUS.HTTP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class HttpServer : IHttpServer
    {
        private readonly IDictionary<string, Func<HttpRequest, HttpResponse>> routeTable
           = new Dictionary<string, Func<HttpRequest, HttpResponse>>();

        public void AddRoute(string path, Func<HttpRequest, HttpResponse> action)
        {
            if (this.routeTable.ContainsKey(path))
            {
                routeTable[path] = action;
            }
            else
            {
                routeTable.Add(path, action);
            }
        }

        public async Task StartAsync(int port)
        {
            var tcpListener =
               new TcpListener(IPAddress.Loopback, port);

            tcpListener.Start();

            while (true)
            {
                var tcpClient = await tcpListener.AcceptTcpClientAsync();

                ProcessClientAsync(tcpClient);
            }
        }

        private async Task ProcessClientAsync(TcpClient tcpClient)
        {
            var stream = tcpClient.GetStream();

            using (stream)
            {
                var data = new List<byte>();
                var buffer = new byte[HttpConstants.BufferSize];
                var position = 0;

                while (true)
                {
                    var count = await stream.ReadAsync(buffer, position, buffer.Length);

                    if (count < buffer.Length)
                    {
                        var partialBuffer = new byte[count];
                        Array.Copy(buffer, partialBuffer, count);
                        data.AddRange(partialBuffer);
                        break;
                    }
                    else
                    {
                        data.AddRange(buffer);
                    }
                }

                var dataAsString = Encoding.UTF8.GetString(data.ToArray());
                var html = $"<h1>Hello from CustomServer {DateTime.Now}</h1>";

                var request = new HttpRequest(dataAsString);

                var responseBodyBytes = Encoding.UTF8.GetBytes(html);
                var response = new HttpResponse("text/html; charset=utf-8", responseBodyBytes);
                response.Headers.Add(new Header("Server", "SUS Server 1.1"));
                response.Cookies.Add(new ResponseCookie("sid", Guid.NewGuid().ToString()) 
                { 
                    HttpOnly = true,
                    MaxAge = 3 * 60 * 60
                });
               
                var responseHeaderBytes = Encoding.UTF8.GetBytes(response.ToString());

                await stream.WriteAsync(responseHeaderBytes, 0, responseHeaderBytes.Length);
                await stream.WriteAsync(response.Body, 0, response.Body.Length);
            }

            tcpClient.Close();
        }
    }
}
