namespace SUS.MvcFramework
{
    using System.Runtime.CompilerServices;
    using System.Text;

    using HTTP;
    using ViewEngine;

    public abstract class Controller
    {
        private const string UserIdSessionName = "UserId";
        private readonly SusViewEngine viewEngine;

        protected Controller()
        {
            this.viewEngine = new SusViewEngine();
        }

        public HttpRequest HttpRequest { get; set; }

        protected HttpResponse View(object viewModel = null, [CallerMemberName] string path = null)
        {
            var main = System.IO.File.ReadAllText("Views/Shared/_Layout.html");
            main = main.Replace("@RenderBody()", "__VIEW__");
            main = viewEngine.GetHtml(main, viewModel, this.GetUserId());

            var viewContent = System.IO.File.ReadAllText("Views/" + this.GetType().Name.Replace("Controller", string.Empty) + "/" + path + ".html");

            viewContent = this.viewEngine.GetHtml(viewContent, viewModel, this.GetUserId());
            var responseHtml = main.Replace("__VIEW__", viewContent);

            var responseBody = Encoding.UTF8.GetBytes(responseHtml);

            var response = new HttpResponse("text/html", responseBody);

            return response;
        }

        protected HttpResponse Redirect(string path)
        {
            var response = new HttpResponse(HttpStatusCode.Found);
            response.Headers.Add(new Header("Location", path));

            return response;
        }

        protected void SignIn(string userId)
            => this.HttpRequest.Session[UserIdSessionName] = userId;

        protected void SignOut()
            => this.HttpRequest.Session[UserIdSessionName] = null;

        protected bool IsUserSignedIn()
            => this.HttpRequest.Session.ContainsKey(UserIdSessionName)
                && this.HttpRequest.Session[UserIdSessionName] != null;

        protected string GetUserId()
            => this.HttpRequest.Session.ContainsKey(UserIdSessionName) ?
               this.HttpRequest.Session[UserIdSessionName] : null;
    }
}
