namespace SUS.MvcFramework
{
    using HTTP;

    public class HttpGetAttribute : BaseHttpAttribute
    {
        public HttpGetAttribute()
        {
        }

        public HttpGetAttribute(string url) 
        {
            this.Url = url;
        }

        public override HttpMethod HttpMethod => HttpMethod.GET;
    }
}
