using System;
using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    public class ToolAssetChannel : IAssetChannel
    {
        public IEnumerable<string> GetAvailableVersions(string assetId)
        {
            EnsureAssetId(assetId);
            return new[] { GetLatestVersion(assetId) };
        }

        public string GetDownloadUrl(string assetId, string version)
        {
            EnsureAssetId(assetId);
            return "https://www.myget.org/F/dnvm/api/v2/package/dnvm.tool.watch/1.0.0-alpha";
        }

        public string GetLatestVersion(string assetId)
        {
            EnsureAssetId(assetId);
            return "1.0.0-alpha";
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
