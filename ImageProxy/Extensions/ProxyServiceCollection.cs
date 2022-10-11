using ImageProxy.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ImageProxy.Extensions;

public static class ProxyServiceCollection
{
    public static IServiceCollection AddImageProxy(this IServiceCollection services)
    {
        services.AddScoped<IProxyService, ProxyService>();
        return services;
    }
}