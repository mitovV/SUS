namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class StaticFilesController : Controller
    {
        public HttpResponse Favico(HttpRequest request)
        {
            return this.File("wwwroot/favicon.ico", "image/x-icon");
        }
    }
}
