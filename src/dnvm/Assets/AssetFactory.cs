using System;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Assets
{
    public class AssetFactory : IAssetFactory
    {
        private readonly IServiceProvider _services;

        public AssetFactory(IServiceProvider serivces)
        {
            _services = serivces;
        }

        public TAsset Create<TAsset>(string version) where TAsset : Asset
        {
            return ActivatorUtilities.CreateInstance<TAsset>(_services, version);
        }
    }
}