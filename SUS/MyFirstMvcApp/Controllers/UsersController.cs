namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class UsersController : Controller
    {
        [HttpGet]
        public HttpResponse Login()
            => this.View();

        [HttpGet]
        public HttpResponse Register()
            => this.View();

        [HttpPost("/users/register")]
        public HttpResponse DoRegister()
            => this.Redirect("/");
    }
}
