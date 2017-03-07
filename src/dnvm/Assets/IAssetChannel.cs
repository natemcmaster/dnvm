using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    public interface IAssetChannel
    {
        IEnumerable<AssetInfo> GetAll(string assetId);
        AssetInfo GetLatest(string assetId);
        AssetInfo GetAssetInfo(string assetId, string version);
    }
}
