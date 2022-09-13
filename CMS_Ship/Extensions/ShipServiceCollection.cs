using System.Reflection;
using CMS_Lib.DI;
using Microsoft.Extensions.DependencyInjection;

namespace CMS_Ship.Extensions;

public static class ShipServiceCollection
{
    public static IServiceCollection AddShipCod(this IServiceCollection services)
    {
        ServiceCollectionExtensions.RegisterAllLib<ITransient>(services,
            typeof(ShipServiceCollection).GetTypeInfo().Assembly);
        ServiceCollectionExtensions.RegisterAllLib<IScoped>(services,
            typeof(ShipServiceCollection).GetTypeInfo().Assembly,ServiceLifetime.Scoped);
        ServiceCollectionExtensions.RegisterAllLib<ISingleton>(services,
            typeof(ShipServiceCollection).GetTypeInfo().Assembly,ServiceLifetime.Singleton);
        return services;
    }
}