using System;
using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    // TODO implement generically, not just for dotnet-watch
    public class ToolAssetChannel : IAssetChannel
    {
        private class ToolAssetInfo : AssetInfo
        {
            public ToolAssetInfo(string name, string version)
            {
                Id = "dnvm.tool." + name;
                Version = version;
                DownloadUrl = $"https://www.myget.org/F/dnvm/api/v2/package/{Id}/{version}";
            }
        }

        public AssetInfo GetAssetInfo(string assetId, string version)
        {
            EnsureAssetId(assetId);
            return new ToolAssetInfo("watch", "1.0.0-alpha");
        }

        public IEnumerable<AssetInfo> GetAll(string assetId)
        {
            EnsureAssetId(assetId);
            return new[] { GetLatest(assetId) };
        }

        public AssetInfo GetLatest(string assetId)
        {
            EnsureAssetId(assetId);
            return new ToolAssetInfo("watch", "1.0.0-alpha");
        }

        private void EnsureAssetId(string assetId)
        {
            if (assetId != "dnvm.tool.watch")
            {
                throw new InvalidOperationException($"Unrecognized asset ID '{assetId}'");
            }
        }
    }
}
