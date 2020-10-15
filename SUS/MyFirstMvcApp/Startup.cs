namespace MyFirstMvcApp
{
    using System.Collections.Generic;

    using SUS.HTTP;
    using SUS.MvcFramework;

    public class Startup : IMvcApplication
    {
        public void Configure(ICollection<Route> routeTable)
        {
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
        }
    }
}
