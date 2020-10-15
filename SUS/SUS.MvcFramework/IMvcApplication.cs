namespace SUS.MvcFramework
{
    using System.Collections.Generic;

    using HTTP;

    public interface IMvcApplication
    {
        void ConfigureServices(IServiceCollection serviceCollection);

        void Configure(ICollection<Route> routeTable);
    }
}
