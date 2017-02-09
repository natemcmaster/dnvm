using Microsoft.Extensions.DependencyInjection;

namespace DotNet
{
    public static class DnvmServiceCollectionExtension
    {
        public static IServiceCollection AddDnvm(this IServiceCollection services)
        {
            return services;
        }
    }
}