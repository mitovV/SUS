namespace MyFirstMvcApp
{
    using System.Collections.Generic;

    using Controllers;
    using SUS.HTTP;
    using SUS.MvcFramework;

    public class Startup : IMvcApplication
    {
        public void Configure(ICollection<Route> routeTable)
        {

            routeTable.Add(new Route("/", HttpMethod.GET, new HomeController().Index));
            routeTable.Add(new Route("/users/login", HttpMethod.GET, new UsersController().Login));
            routeTable.Add(new Route("/users/register", HttpMethod.GET, new UsersController().Register));
            routeTable.Add(new Route("/users/register", HttpMethod.POST, new UsersController().DoRegister));
            routeTable.Add(new Route("/favicon.ico", HttpMethod.GET, new StaticFilesController().Favico));
            routeTable.Add(new Route("/wwwroot/css/site.css", HttpMethod.GET, new StaticFilesController().SiteCss));
        }

        public void ConfigureServices()
        {
        }
    }
}
