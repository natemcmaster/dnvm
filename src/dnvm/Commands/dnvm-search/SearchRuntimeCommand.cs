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
            var stable = channel.GetLatest(RuntimeAsset.CreateAssetId(Architecture.X64)).Version;
            foreach (var runtime in channel.GetAll(RuntimeAsset.CreateAssetId(Architecture.X64)))
            {
                var version = runtime.Version;
                var line = version == stable
                ? $"{version} ({SdkAsset.DefaultVersion})"
                : version;
                var installed = context.Environment
                    .Runtimes
                    .Any(f => f.Name == RuntimeAsset.CreateAssetId(Architecture.X64) && f.Version.Equals(version))
                    ? "*"
                    : " ";

                context.Logger.Output($"    {installed} {line}");
            }

            context.Result = Result.Okay;
        }
    }
}
