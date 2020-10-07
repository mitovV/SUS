namespace SUS.HTTP
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class HttpResponse
    {
        public HttpResponse(HttpStatusCode statusCode)
        {
            this.StatusCode = statusCode;
            this.Headers = new List<Header>();
            this.Cookies = new List<Cookie>();
            this.Body = new byte[0];
        }

        public HttpResponse(string contentType, byte[] body, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            this.StatusCode = statusCode;

            if (body == null)
            {
                body = new byte[0];
            }

            this.Body = body;
            this.Headers = new List<Header>();
            this.Cookies = new List<Cookie>();
            this.Headers.Add(new Header("Content-Type", contentType));
            this.Headers.Add(new Header("Content-Length", body.Length.ToString()));
        }

        public HttpStatusCode StatusCode { get; set; }

        public ICollection<Header> Headers { get; set; }

        public ICollection<Cookie> Cookies { get; set; }

        public byte[] Body { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append($"HTTP/1.1 {(int)this.StatusCode} {this.StatusCode}" + HttpConstants.NewLine);

            foreach (var header in this.Headers)
            {
                sb.Append(header.ToString() + HttpConstants.NewLine);
            }

            if (this.Cookies.Any())
            {
                foreach (var cookie in this.Cookies)
                {
                    sb.Append($"Set-Cookie: {cookie}" + HttpConstants.NewLine);
                }
            }

            sb.Append(HttpConstants.NewLine);

            return sb.ToString();
        }
    }
}
