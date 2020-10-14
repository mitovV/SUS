namespace SUS.HTTP
{
    using System.Text;

    public class ResponseCookie : Cookie
    {
        public ResponseCookie(string name, string value)
            : base(name, value)
        { }

        public long MaxAge { get; set; }

        public bool HttpOnly { get; set; }

        public string Path { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"{this.Name}={this.Value}");

            if (this.Path != null)
            {
                sb.Append($"; Path={this.Path}");
            }

            if (MaxAge != 0)
            {
                sb.Append($"; Max-Age={this.MaxAge}");
            }


            if (HttpOnly)
            {
                sb.Append($"; HttpOnly");
            }

            return sb.ToString();
        }
    }
}
