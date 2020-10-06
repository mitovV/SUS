namespace SUS.MvcFramework
{
    using System.Runtime.CompilerServices;
    using System.Text;

    using SUS.HTTP;

    public abstract class Controller
    {
        public HttpResponse View([CallerMemberName] string path = null)
        {
            var main = System.IO.File.ReadAllText("Views/Shared/_Layout.html");

            var viewContent = System.IO.File.ReadAllText("Views/" + this.GetType().Name.Replace("Controller", string.Empty) + "/" + path + ".html");

           var  responseHtml = main.Replace("@RenderBody()", viewContent);

            var responseBody = Encoding.UTF8.GetBytes(responseHtml);

            var response = new HttpResponse("text/html", responseBody);

            return response;
        }

        public HttpResponse File(string path, string type)
        {
            var fileBytes = System.IO.File.ReadAllBytes(path);

            var response = new HttpResponse(type, fileBytes);

            return response;
        }

        public HttpResponse Redirect(string path)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            response.Headers.Add(new Header("Location", path));

            return response;
        }
    }
}
