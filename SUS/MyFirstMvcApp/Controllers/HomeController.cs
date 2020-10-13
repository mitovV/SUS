namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class HomeController : Controller
    {
        [HttpGet("/")]
        public HttpResponse Index()
            => this.View();
    }
}
