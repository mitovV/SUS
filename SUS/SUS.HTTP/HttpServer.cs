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
        private readonly ICollection<Route> routeTable;

        public HttpServer(ICollection<Route> routeTable)
        {
            this.routeTable = routeTable;
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

                var requestAsString = Encoding.UTF8.GetString(data.ToArray());

                var request = new HttpRequest(requestAsString);
                Console.WriteLine($"{request.Method} {request.Path} => {request.Headers.Count} headers");

                HttpResponse response;

                var route = this.routeTable.FirstOrDefault(x => string.Compare( x.Path, request.Path, true) == 0 && x.Method == request.Method);

                if (route != null)
                {
                    response = route.Action(request);
                }
                else
                {
                    response = new HttpResponse("text/html", null, HttpStatusCode.NotFound);
                }

                response.Headers.Add(new Header("Server", "SUS Server 1.1"));
                var sessionCookie = request.Cookies.FirstOrDefault(x => x.Name == HttpConstants.SessionCookieName);

                if (sessionCookie != null)
                {
                    var responseCookie = new ResponseCookie(sessionCookie.Name, sessionCookie.Value)
                    {
                        Path = "/"
                    };
                    response.Cookies.Add(responseCookie);
                }

                var responseHeaderBytes = Encoding.UTF8.GetBytes(response.ToString());

                await stream.WriteAsync(responseHeaderBytes, 0, responseHeaderBytes.Length);
                await stream.WriteAsync(response.Body, 0, response.Body.Length);
            }

            tcpClient.Close();
        }
    }
}