namespace SUS.HTTP
{
    using System.Collections.Generic;

    public class HttpRequest
    {
        public HttpRequest(string requesString)
        {
            this.Cookies = new List<Cookie>();
            this.Headers = new List<Header>();

            var lines = requesString.Split(new string[] { HttpConstants.NewLine }, System.StringSplitOptions.None);
        }

        public string Path { get; set; }

        public HttpMethod Method { get; set; }

        public ICollection<Header> Headers { get; set; }

        public ICollection<Cookie> Cookies { get; set; }

        public string Body { get; set; }
    }
}
