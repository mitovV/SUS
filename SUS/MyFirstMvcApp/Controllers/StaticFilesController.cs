namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class StaticFilesController : Controller
    {
        public HttpResponse Favico(HttpRequest request)
            => this.File("wwwroot/favicon.ico", "image/x-icon");

        public HttpResponse SiteCss(HttpRequest request)
            => this.File("wwwroot/css/site.css", "text/css");
    }
}
