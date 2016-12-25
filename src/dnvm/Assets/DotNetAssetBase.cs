using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Reporting;
using DotNet.Utils;
using Microsoft.DotNet.PlatformAbstractions;

namespace DotNet.Assets
{

    public abstract class DotNetAssetBase : Asset
    {
        protected const string AzureFeed = "https://dotnetcli.blob.core.windows.net/dotnet";

        private static readonly HttpClient DefaultHttpClient = new HttpClient();
        protected IReporter Reporter { get; }

        public DotNetAssetBase(IReporter reporter)
        {
            Reporter = reporter;
        }

        protected async Task<bool> DownloadAndExtractAsync(string url, string destination, CancellationToken cancellationToken)
        {
            Reporter.Verbose($"Downloading from '{url}'");

            var result = await DefaultHttpClient.GetAsync(url, cancellationToken);
            if (!result.IsSuccessStatusCode)
            {
                Reporter.Error($"Failed to download '{url}'");
                throw new InvalidOperationException("Installation failed");
            }

            var filename = Path.GetFileName(url);
            using (var tmp = new TempFile())
            using (var stream = new FileStream(tmp.Path, FileMode.Create))
            {
                await result.Content.CopyToAsync(stream);

                Reporter.Verbose($"Extracting '{filename}' to '{destination}'");

                if (filename.EndsWith(".tar.gz") || filename.EndsWith(".tgz"))
                {
                    return TarballExtractor.Extract(tmp.Path, destination, gzipped: true);
                }
                else
                {
                    throw new InvalidOperationException("Unrecognized file extension");
                }
            }
        }

        protected static string GetRid()
        {
            if (RuntimeEnvironment.OperatingSystemPlatform == Platform.Darwin)
            {
                return "osx-x64"; // RID includes OS version, but shared fx is not version specific
            }
            return RuntimeEnvironment.GetRuntimeIdentifier();
        }
    }
}