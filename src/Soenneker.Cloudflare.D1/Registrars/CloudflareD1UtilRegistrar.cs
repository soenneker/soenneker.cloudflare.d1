using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Soenneker.Cloudflare.D1.Abstract;
using Soenneker.Cloudflare.Utils.Client.Registrars;

namespace Soenneker.Cloudflare.D1.Registrars;

/// <summary>
/// A utility library for Cloudflare D1 database operations.
/// </summary>
public static class CloudflareD1UtilRegistrar
{
    /// <summary>
    /// Adds <see cref="ICloudflareD1Util"/> as a singleton service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareD1UtilAsSingleton(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddSingleton<ICloudflareD1Util, CloudflareD1Util>();

        return services;
    }

    /// <summary>
    /// Adds <see cref="ICloudflareD1Util"/> as a scoped service. <para/>
    /// </summary>
    public static IServiceCollection AddCloudflareD1UtilAsScoped(this IServiceCollection services)
    {
        services.AddCloudflareClientUtilAsSingleton().TryAddScoped<ICloudflareD1Util, CloudflareD1Util>();

        return services;
    }
}
