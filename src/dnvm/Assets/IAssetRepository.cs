using System.Collections.Generic;

namespace DotNet.Assets
{
    public interface IAssetRepository
    {
        IEnumerable<string> ListAvailableVersions(string assetId);
        string GetLatestVersion(string assetId);
        string GetDownloadUrl(string assetId, string version);
    }
}