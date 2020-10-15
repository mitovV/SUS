namespace SUS.MvcFramework
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ServiceCollection : IServiceCollection
    {
        private readonly IDictionary<Type, Type> dependencyCountainer
            = new Dictionary<Type, Type>();

        public void Add<TSource, TDestination>()
            => this.dependencyCountainer.Add(typeof(TSource), typeof(TDestination));

        public object CreateInstance(Type type)
        {
            if (this.dependencyCountainer.ContainsKey(type))
            {
                type = this.dependencyCountainer[type];
            }

            var constructor = type
                .GetConstructors()
                .OrderBy(x => x.GetParameters().Count())
                .FirstOrDefault();

            var parameterValues = new List<object>();

            var parameters = constructor.GetParameters();

            foreach (var parameter in parameters)
            {
                var instance = CreateInstance(parameter.ParameterType);
                parameterValues.Add(instance);
            }

            var result = constructor.Invoke(parameterValues.ToArray());

            return result;
        }
    }
}
