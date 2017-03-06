using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.VersionManager.Utils;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Assets
{
    public abstract class AssetBase : Asset
    {
        protected ILogger Log { get; }

        public AssetBase(ILogger logger)
        {
            Log = logger;
        }

        protected bool UninstallFolder(string path)
        {
            if (Directory.Exists(path))
            {
                Log.Verbose($"Deleting '{path}'");
                try
                {
                    Directory.Delete(path, recursive: true);
                }
                catch (Exception ex)
                {
                    Log.Verbose(ex.Message);
                    Log.Error($"Failed to delete '{path}'. Try again or remove manually.");
                    return false;
                }
                Log.Verbose("Deleted");
            }
            else
            {
                Log.Verbose($"Skipping. '{path}' does not exist.");
            }
            return true;
        }

        protected Task<bool> DownloadAndExtractAsync(string url,
           string destination,
           CancellationToken cancellationToken)
           => DownloadAndExtractAsync(url, destination, ArchiveUtility.Extract, cancellationToken);

        protected delegate bool ArchiveExtractor(string archiveFile, string desination);

        protected async Task<bool> DownloadAndExtractAsync(string url,
            string destination,
            ArchiveExtractor extractor,
            CancellationToken cancellationToken)
        {
            Log.Verbose($"Downloading from '{url}'");

            HttpResponseMessage result;
            try
            {
                result = await Http.DefaultHttpClient.GetAsync(url, cancellationToken);
            }
            catch (HttpRequestException ex)
            {
                Log.Error($"Downloading asset failed: {ex.Message}");
                return false;
            }

            if (!result.IsSuccessStatusCode)
            {
                Log.Error($"Failed to download '{url}'");
                throw new InvalidOperationException("Installation failed");
            }

            var filename = Path.GetFileName(url);
            var ext = filename.EndsWith(".tar.gz", StringComparison.OrdinalIgnoreCase)
                ? ".tar.gz"
                : Path.GetExtension(filename);

            using (var tmp = new TempFile(ext))
            using (var stream = new FileStream(tmp.Path, FileMode.Create))
            {
                await result.Content.CopyToAsync(stream);

                Log.Verbose($"Extracting '{filename}' to '{destination}'");

                return extractor(tmp.Path, destination);
            }
        }
    }
}
