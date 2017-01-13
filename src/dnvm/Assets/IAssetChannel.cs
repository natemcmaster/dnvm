using System.Collections.Generic;

namespace DotNet.Assets
{
    public interface IAssetChannel
    {
        IEnumerable<string> ListAvailableVersions(string assetId);
        string GetLatestVersion(string assetId);
        string GetDownloadUrl(string assetId, string version);
    }
}