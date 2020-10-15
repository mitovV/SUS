namespace SUS.MvcFramework
{
    using System;

    public interface IServiceCollection
    {
        void Add<TSource, TDestination>();

        object CreateInstance(Type type);
    }
}
