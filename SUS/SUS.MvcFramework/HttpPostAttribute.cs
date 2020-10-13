namespace SUS.MvcFramework
{
    using HTTP;

    public class HttpPostAttribute : BaseHttpAttribute
    {
        public HttpPostAttribute(string url) 
        {
            this.Url = url;
        }

        public override HttpMethod HttpMethod => HttpMethod.POST;
    }
}
