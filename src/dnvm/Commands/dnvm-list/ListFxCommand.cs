using DotNet.Assets;

namespace DotNet.Commands
{
    public class ListFxCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            var channel = new StableAssetChannel();
            var stable = channel.GetLatestVersion(SharedFxAsset.AssetId);
            foreach (var version in channel.GetAvailableVersions(SharedFxAsset.AssetId))
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