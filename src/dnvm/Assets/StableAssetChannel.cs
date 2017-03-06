using System;
using System.Collections.Generic;
using System.Linq;

namespace DotNet.VersionManager.Assets
{
    public partial class StableAssetChannel : IAssetChannel
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

        public IEnumerable<string> GetAvailableVersions(string assetId)
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
            {
                if (!Versions.TryGetValue(version, out var url))
                {
                    throw new KeyNotFoundException($"Unrecognized version '{version}'");
                }
                return url;
            }

            public IEnumerable<string> GetVersions()
                => Versions.Keys;
        }
    }
}
