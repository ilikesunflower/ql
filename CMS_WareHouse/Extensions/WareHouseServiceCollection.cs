using System.Reflection;
using CMS_Lib.DI;
using Microsoft.Extensions.DependencyInjection;

namespace CMS_WareHouse.Extensions;

public static class WareHouseServiceCollection
{
    public static IServiceCollection AddWareHouse(this IServiceCollection services)
    {
        ServiceCollectionExtensions.RegisterAllLib<ITransient>(services,
            typeof(WareHouseServiceCollection).GetTypeInfo().Assembly);
        ServiceCollectionExtensions.RegisterAllLib<IScoped>(services,
            typeof(WareHouseServiceCollection).GetTypeInfo().Assembly,ServiceLifetime.Scoped);
        ServiceCollectionExtensions.RegisterAllLib<ISingleton>(services,
            typeof(WareHouseServiceCollection).GetTypeInfo().Assembly,ServiceLifetime.Singleton);
        return services;
    }
}