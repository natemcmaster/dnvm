using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    public abstract class DotNetAssetBase : AssetBase
    {
        public DotNetAssetBase(ILogger logger)
        : base(logger)
        {
        }

        protected IAssetChannel Channel { get; } = new StableAssetChannel();
    }
}
