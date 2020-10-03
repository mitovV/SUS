namespace MyFirstMvcApp.Controllers
{
    using System.IO;
    using System.Text;

    using SUS.HTTP;
    using SUS.MvcFramework;

    public class HomeController : Controller
    {
        public HttpResponse Index(HttpRequest request)
        {
            var responseText = File.ReadAllText("Views/Home/Index.html");
            var responseBody = Encoding.UTF8.GetBytes(responseText);

            var response = new HttpResponse("text/html", responseBody);

            return response;
        }
    }
}
