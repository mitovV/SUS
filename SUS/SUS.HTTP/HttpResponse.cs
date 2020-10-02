namespace SUS.HTTP
{
    using System.Collections.Generic;

    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public HttpMethod Method { get; set; }

        public ICollection<Header> Headers { get; set; }

        public ICollection<Cookie> Cookies { get; set; }

        public byte[] Body { get; set; }
    }
}
