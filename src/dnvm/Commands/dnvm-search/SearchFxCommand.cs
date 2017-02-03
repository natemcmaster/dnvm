using System.Linq;
using DotNet.Assets;
using DotNet.Reporting;

namespace DotNet.Commands
{
    public class SearchFxCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            context.Reporter.Output("*=installed");
            context.Reporter.Output();

            using (context.Indent())
            {
                var channel = new StableAssetChannel();
                var stable = channel.GetLatestVersion(SharedFxAsset.AssetId);
                foreach (var version in channel.GetAvailableVersions(SharedFxAsset.AssetId))
                {
                    var line = version == stable
                    ? $"{version} ({SdkAsset.DefaultVersion})"
                    : version;
                    var installed = context.Environment
                        .Frameworks
                        .Any(f => f.Name == SharedFxAsset.AssetId && f.Version.Equals(version))
                        ? "*"
                        : " ";

                    context.Reporter.Output($"{installed} {line}");
                }
            }

            context.Result = Result.Okay;
        }
    }
}