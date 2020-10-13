namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class UsersController : Controller
    {
        [HttpGet]
        public HttpResponse Login(HttpRequest request)
            => this.View();

        [HttpGet]
        public HttpResponse Register(HttpRequest request)
            => this.View();

        [HttpPost("users/register")]
        public HttpResponse DoRegister(HttpRequest request)
            => this.Redirect("/");
    }
}
