namespace SUS.HTTP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;

    public class HttpRequest
    {
        public HttpRequest(string requesString)
        {
            this.Cookies = new List<Cookie>();
            this.Headers = new List<Header>();
            this.FormData = new Dictionary<string, string>();

            var lines = requesString.Split(new string[] { HttpConstants.NewLine }, StringSplitOptions.None);

            var headerLine = lines[0];

            var headerLineParts = headerLine.Split(' ');

            this.Method = (HttpMethod)Enum.Parse(typeof(HttpMethod), headerLineParts[0]);
            this.Path = headerLineParts[1];

            var isInHeaders = true;
            var bodyBuilder = new StringBuilder();

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];

                if (string.IsNullOrWhiteSpace(line))
                {
                    isInHeaders = false;
                    continue;
                }

                if (isInHeaders)
                {
                    this.Headers.Add(new Header(line));
                }
                else
                {
                    bodyBuilder.AppendLine(line);
                }
            }

            if (this.Headers.Any(x => x.Name == HttpConstants.RequestCookieHeader))
            {
                var cookiesAsString = this.Headers.FirstOrDefault(x => x.Name == HttpConstants.RequestCookieHeader).Value;

                var cookies = cookiesAsString.Split(new string[] { "; " }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var cookie in cookies)
                {
                    this.Cookies.Add(new Cookie(cookie));
                }
            }

            this.Body = bodyBuilder.ToString();

            var parameters = this.Body.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var parameter in parameters)
            {
                var parameterParts = parameter.Split(new string[] { "=" }, StringSplitOptions.RemoveEmptyEntries);

                var name = parameterParts[0];
                var value = WebUtility.UrlEncode(parameterParts[1]);

                if (!this.FormData.ContainsKey(name))
                {
                    this.FormData.Add(name, value);
                }
            }
        }

        public string Path { get; set; }

        public IDictionary<string, string> FormData { get; set; }

        public HttpMethod Method { get; set; }

        public ICollection<Header> Headers { get; set; }

        public ICollection<Cookie> Cookies { get; set; }

        public string Body { get; set; }
    }
}
