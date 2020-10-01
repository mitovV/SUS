namespace SUS.HTTP
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading.Tasks;

    public class HttpServer : IHttpServer
    {
        private readonly IDictionary<string, Func<HttpRequest, HttpRespose>> routeTable
           = new Dictionary<string, Func<HttpRequest, HttpRespose>>();

        public void AddRoute(string path, Func<HttpRequest, HttpRespose> action)
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
            }

            tcpClient.Close();
        }
    }
}
