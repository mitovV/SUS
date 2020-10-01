namespace SUS.HTTP
{
    using System.Collections.Generic;

    public class HttpRequest
    {
        public ICollection<Header> Headers { get; set; }

        public string Body { get; set; }
    }
}
