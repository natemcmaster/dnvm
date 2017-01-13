
using System.Linq;
using DotNet.Assets;
using DotNet.Reporting;

namespace DotNet.Commands
{
    public class ListSdkCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            context.Reporter.Output("*=installed");
            context.Reporter.Output();

            using (context.Indent())
            {
                var channel = new StableAssetChannel();
                var stable = channel.GetLatestVersion(SdkAsset.AssetId);
                foreach (var version in channel.GetAvailableVersions(SdkAsset.AssetId))
                {
                    var line = version == stable
                    ? $"{version} ({SdkAsset.DefaultVersion})"
                    : version;
                    var installed = context.Environment
                        .Sdks
                        .Any(s => s.Version.Equals(version))
                        ? "*"
                        : " ";

                    context.Reporter.Output($"{installed} {line}");
                }
            }

            context.Result = Result.Okay;
        }
    }
}