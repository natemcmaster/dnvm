using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    public interface IAssetChannel
    {
        IEnumerable<string> GetAvailableVersions(string assetId);
        string GetLatestVersion(string assetId);
        string GetDownloadUrl(string assetId, string version);
    }
}
