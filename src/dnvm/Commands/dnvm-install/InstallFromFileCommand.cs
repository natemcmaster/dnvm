using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    class InstallFromFileCommand : InstallCommandBase
    {
        public override async Task ExecuteAsync(CommandContext context)
        {
            if (context.ConfigFile == null)
            {
                context.Logger.Error("No config file could be found.");
                context.Logger.Output("Try executing `dnvm init` to create a config file, or use a subcommand of `dnvm install`.");
                context.Logger.Output("See `dnvm install --help` for more info.");
                context.Result = Result.Error;
            }
            else
            {
                await base.ExecuteAsync(context);
            }
        }

        protected override IEnumerable<Asset> CreateAssets(CommandContext context)
        {
            if (!string.IsNullOrEmpty(context.ConfigFile.Sdk))
            {
                yield return new SdkAsset(context.Logger, context.Environment, context.ConfigFile.Sdk, Architecture.X64);
            }

            foreach (var runtime in context.ConfigFile.Runtimes)
            {
                yield return new RuntimeAsset(context.Logger, context.Environment, runtime, Architecture.X64);
            }

            foreach (var tool in context.ConfigFile.Tools)
            {
                yield return new ToolAsset(context.Logger, context.Environment, tool.Key, tool.Value);
            }
        }
    }
}
