namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class HomeController : Controller
    {
        public HttpResponse Index(HttpRequest request)
        => this.View();
    }
}
