namespace SUS.MvcFramework
{
    using System.Runtime.CompilerServices;
    using System.Text;

    using SUS.HTTP;
    using ViewEngine;

    public abstract class Controller
    {
        private readonly SusViewEngine viewEngine;

        public Controller()
        {
            this.viewEngine = new SusViewEngine();
        }

        public HttpRequest HttpRequest { get; set; }

        public HttpResponse View(object viewModel = null, [CallerMemberName] string path = null)
        {
            var main = System.IO.File.ReadAllText("Views/Shared/_Layout.html");
            main = main.Replace("@RenderBody()", "__VIEW__");
            main = viewEngine.GetHtml(main, viewModel);

            var viewContent = System.IO.File.ReadAllText("Views/" + this.GetType().Name.Replace("Controller", string.Empty) + "/" + path + ".html");

            viewContent = this.viewEngine.GetHtml(viewContent, viewModel);
            var responseHtml = main.Replace("__VIEW__", viewContent);

            var responseBody = Encoding.UTF8.GetBytes(responseHtml);

            var response = new HttpResponse("text/html", responseBody);

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
