namespace SUS.MvcFramework
{
    using System;

    using HTTP;

    public abstract class BaseHttpAttribute : Attribute
    {
        public string Url { get; set; }

        public abstract HttpMethod HttpMethod { get; }
    }
}
