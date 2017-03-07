using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.VersionManager.Assets
{
    public partial class StableAssetChannel : IAssetChannel
    {
        public AssetInfo GetLatest(string assetId)
            => GetAll(assetId).Last();

        public IEnumerable<AssetInfo> GetAll(string assetId)
        {
            if (!_assets.TryGetValue(assetId, out var collection))
            {
                throw new InvalidOperationException("Unrecognized asset ID");
            }
            return collection;
        }

        public AssetInfo GetAssetInfo(string assetId, string version)
        {
            if (!_assets.TryGetValue(assetId, out var collection))
            {
                throw new InvalidOperationException("Unrecognized asset ID");
            }
            return collection.Single(v => v.Version == version);
        }
    }
}
