namespace SUS.MvcFramework
{
    using System.Runtime.CompilerServices;
    using System.Text;

    using SUS.HTTP;

    public abstract class Controller
    {
        public HttpResponse View([CallerMemberName] string path = null)
        {
            var responseText = System.IO.File.ReadAllText("Views/" + this.GetType().Name.Replace("Controller", string.Empty) + "/" + path + ".html");
            var responseBody = Encoding.UTF8.GetBytes(responseText);

            var response = new HttpResponse("text/html", responseBody);

            return response;
        }

        public HttpResponse File(string path, string type)
        {
            var fileBytes = System.IO.File.ReadAllBytes(path);

            var response = new HttpResponse(type, fileBytes);

            return response;
        }
    }
}
