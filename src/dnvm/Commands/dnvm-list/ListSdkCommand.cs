
using DotNet.Assets;

namespace DotNet.Commands
{
    public class ListSdkCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            var channel = new StableAssetChannel();
            var stable = channel.GetLatestVersion(SdkAsset.AssetId);
            foreach (var version in channel.GetAvailableVersions(SdkAsset.AssetId))
            {
                var line = version == stable
                    ? $"{version} ({SdkAsset.DefaultVersion})"
                    : version;
                context.Reporter.Output(line);
            }

            context.Result = Result.Okay;
        }
    }
}