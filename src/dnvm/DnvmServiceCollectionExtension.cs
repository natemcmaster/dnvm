using DotNet.Assets;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet
{
    public static class DnvmServiceCollectionExtension
    {
        public static IServiceCollection AddDnvm(this IServiceCollection services)
        {
            services
                .AddSingleton<IAssetFactory, AssetFactory>();

            return services;
        }
    }
}