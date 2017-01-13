using DotNet.Reporting;

namespace DotNet.Assets
{
    public abstract class DotNetAssetBase : AssetBase
    {
        public DotNetAssetBase(IReporter reporter)
        : base(reporter)
        {
        }

        protected IAssetChannel Channel { get; } = new StableAssetChannel();
    }
}