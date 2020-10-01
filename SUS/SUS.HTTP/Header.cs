namespace SUS.HTTP
{
    public class Header
    {
        public Header(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }

        public Header(string headerLine)
        {
            var headerParts = headerLine.Split(new char[] { ' ' }, 2);
            this.Name = headerParts[0];
            this.Value = headerParts[1];
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public override string ToString()
         => $"{this.Name}: {this.Value}";
    }
}
