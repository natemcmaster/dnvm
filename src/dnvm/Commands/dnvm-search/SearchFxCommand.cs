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
                var stable = channel.GetLatestVersion(SharedFxAsset._assetId);
                foreach (var version in channel.GetAvailableVersions(SharedFxAsset._assetId))
                {
                    var line = version == stable
                    ? $"{version} ({SdkAsset.DefaultVersion})"
                    : version;
                    var installed = context.Environment
                        .Frameworks
                        .Any(f => f.Name == SharedFxAsset._assetId && f.Version.Equals(version))
                        ? "*"
                        : " ";

                    context.Reporter.Output($"{installed} {line}");
                }
            }

            context.Result = Result.Okay;
        }
    }
}