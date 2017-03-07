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
            var stable = channel.GetLatest(SdkAsset.CreateAssetId(Architecture.X64)).Version;
            foreach (var sdk in channel.GetAll(SdkAsset.CreateAssetId(Architecture.X64)))
            {
                var version = sdk.Version;
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
