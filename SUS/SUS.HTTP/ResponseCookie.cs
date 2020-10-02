using System.Text;

namespace SUS.HTTP
{
    public class ResponseCookie : Cookie
    {
        public ResponseCookie(string name, string value)
            : base(name, value)
        { }

        public long MaxAge { get; set; }

        public bool HttpOnly { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();

            sb.Append($"{this.Name}={this.Value}");

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
