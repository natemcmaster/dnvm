using System.Linq;
using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class SearchSdkCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            context.Logger.Output("*=installed");
            context.Logger.Output("");

            var channel = new StableAssetChannel();
            var stable = channel.GetLatestVersion(SdkAsset.GetAssetId(Architecture.X64));
            foreach (var version in channel.GetAvailableVersions(SdkAsset.GetAssetId(Architecture.X64)))
            {
                var line = version == stable
                ? $"{version} ({SdkAsset.DefaultVersion})"
                : version;
                var installed = context.Environment
                    .Sdks
                    .Any(s => s.Version.Equals(version))
                    ? "*"
                    : " ";

                context.Logger.Output($"    {installed} {line}");
            }

            context.Result = Result.Okay;
        }
    }
}
