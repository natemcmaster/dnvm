using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.Assets
{
    public partial class StableReleasesAssetRepository : IAssetRepository
    {
        public string GetDownloadUrl(string assetId, string version)
        {
            if (!_assets.TryGetValue(assetId, out var info))
            {
                throw new InvalidOperationException("Unrecognized asset ID");
            }
            return info.GetDownloadUrl(version);
        }

        public string GetLatestVersion(string assetId)
        {
            if (!_assets.TryGetValue(assetId, out var info))
            {
                throw new InvalidOperationException("Unrecognized asset ID");
            }
            return info.Versions.Last().Key;
        }

        public IEnumerable<string> ListAvailableVersions(string assetId)
        {
            if (!_assets.TryGetValue(assetId, out var info))
            {
                throw new InvalidOperationException("Unrecognized asset ID");
            }
            return info.GetVersions();
        }

        private class AssetInfo
        {
            public IDictionary<string, string> Versions { get; }
                = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            public string GetDownloadUrl(string version)
                => Versions[version];

            public IEnumerable<string> GetVersions() 
                => Versions.Keys;
        }
    }
}