namespace MyFirstMvcApp.Controllers
{
    using SUS.HTTP;
    using SUS.MvcFramework;

    using System.IO;

    public class StaticFilesController : Controller
    {
        public HttpResponse Favico(HttpRequest request)
        {
            var fileBytes = File.ReadAllBytes("wwwroot/favicon.ico");

            var response = new HttpResponse("image/x-icon", fileBytes);

            return response;
        }
    }
}
