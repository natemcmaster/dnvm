using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DotNet.VersionManager.Commands
{
    class InstallFromFileCommand : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
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
                var commands = new List<ICommand>();
                if (!string.IsNullOrEmpty(context.ConfigFile.Sdk))
                {
                    commands.Add(new InstallSdkCommand(context.ConfigFile.Sdk, Architecture.X64));
                }

                foreach (var runtime in context.ConfigFile.Runtime)
                {
                    commands.Add(new InstallRuntimeCommand(runtime, Architecture.X64));
                }

                foreach (var tool in context.ConfigFile.Tools)
                {
                    commands.Add(new InstallToolCommand(tool.Key, tool.Value));
                }

                if (commands.Count == 0)
                {
                    context.Logger.Warn("Nothing will be installed because the config file does not list assets.");
                }

                var composite = CommonCommands.Sequence(commands);

                await composite.ExecuteAsync(context);

                if (context.Result != Result.Error)
                {
                    context.Result = Result.Okay;
                }
            }
        }
    }
}
