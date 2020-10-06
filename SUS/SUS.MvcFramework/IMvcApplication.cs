namespace SUS.MvcFramework
{
    using System.Collections.Generic;

    using HTTP;

    public interface IMvcApplication
    {
        void ConfigureServices();

        void Configure(ICollection<Route> routeTable);
    }
}
