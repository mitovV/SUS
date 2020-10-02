namespace SUS.HTTP
{
    using System;
    using System.Collections.Generic;
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
                var response = "HTTP/1.1 200 OK" + HttpConstants.NewLine +
                    "Server: CustomServer 2020" + HttpConstants.NewLine +
                    // "Location: https://google.com" + NewLine +
                    "Content-Type: text/html; charset=utf-8" + HttpConstants.NewLine +
                    "Set-Cookie: language=bg" + HttpConstants.NewLine +
                    "Set-Cookie: sid=12345ggj; Secure; HttpOnly" + HttpConstants.NewLine +
                    //"Set-Cookie: test=value; Max-Age=" + 20 + NewLine +
                    //"Set-Cookie: test=pathCookie; Path=/test" + NewLine +
                    $"Content-Length: {html.Length}" + HttpConstants.NewLine +
                    HttpConstants.NewLine +
                    html +
                    HttpConstants.NewLine;

                var responseBytes = Encoding.UTF8.GetBytes(response);

                await stream.WriteAsync(responseBytes,0, responseBytes.Length);
            }

            tcpClient.Close();
        }
    }
}
