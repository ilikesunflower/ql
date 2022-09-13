using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace CMS_Lib.DI
{
    public class ServiceCollectionExtensions
    {
        public static void RegisterAllType<T>(IServiceCollection serviceCollection, Assembly[] assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.ImplementedInterfaces.Contains(typeof(T)) && x.IsInterface));
            var fromAssemblies = typesFromAssemblies as TypeInfo[] ?? typesFromAssemblies.ToArray();
            if (fromAssemblies.Any())
            {
                foreach (var type in fromAssemblies)
                {
                    var typesClass = assemblies.Select(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(type) && x.IsClass)).FirstOrDefault();
                    if (typesClass != null)
                    {
                        serviceCollection.Add(new ServiceDescriptor(type, typesClass.FirstOrDefault()!, lifetime));
                    }
                }
            }
        }
        public static void RegisterAllLib<T>(IServiceCollection serviceCollection, Assembly assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies = assemblies.DefinedTypes.Where(x => x.ImplementedInterfaces.Contains(typeof(T)) && x.IsInterface);
            var fromAssemblies = typesFromAssemblies as TypeInfo[] ?? typesFromAssemblies.ToArray();
            if (fromAssemblies.Any())
            {
                foreach (var type in fromAssemblies)
                {
                    var typesClass = assemblies.DefinedTypes.FirstOrDefault(x => x.GetInterfaces().Contains(type) && x.IsClass);
                    if (typesClass != null)
                    {
                        serviceCollection.Add(new ServiceDescriptor(type, typesClass, lifetime));
                    }
                }
            }
        }
    }
}
