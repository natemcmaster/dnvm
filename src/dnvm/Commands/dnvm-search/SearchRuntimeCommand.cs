using System.Linq;
using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class SearchRuntimeCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            context.Logger.Output("*=installed");
            context.Logger.Output("");

            var channel = new StableAssetChannel();
            var stable = channel.GetLatestVersion(RuntimeAsset.GetAssetId(Architecture.X64));
            foreach (var version in channel.GetAvailableVersions(RuntimeAsset.GetAssetId(Architecture.X64)))
            {
                var line = version == stable
                ? $"{version} ({SdkAsset.DefaultVersion})"
                : version;
                var installed = context.Environment
                    .Runtimes
                    .Any(f => f.Name == RuntimeAsset.GetAssetId(Architecture.X64) && f.Version.Equals(version))
                    ? "*"
                    : " ";

                context.Logger.Output($"    {installed} {line}");
            }

            context.Result = Result.Okay;
        }
    }
}
