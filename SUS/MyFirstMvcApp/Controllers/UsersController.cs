namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    using System;

    public class UsersController : Controller
    {
        public HttpResponse Login(HttpRequest request)
        => this.View();

        public HttpResponse Register(HttpRequest request)
        => this.View();

        public HttpResponse DoRegister(HttpRequest arg)
        => this.Redirect("/");
    }
}
