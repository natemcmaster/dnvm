using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet.Assets;

namespace DotNet.Commands
{
    class InstallFromFileCommand : ICommand
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            if (context.ConfigFile == null)
            {
                context.Reporter.Error("No config file could be found.");
                context.Reporter.Output("Try executing `dnvm init` to create a config file, or use a subcommand of `dnvm install`.");
                context.Reporter.Output("See `dnvm install --help` for more info.");
                context.Result = Result.Error;
            }
            else
            {
                var commands = new List<ICommand>();
                if (!string.IsNullOrEmpty(context.ConfigFile.Sdk))
                {
                    commands.Add(new InstallCommand<SdkAsset>(context.ConfigFile.Sdk));
                }

                foreach (var fx in context.ConfigFile.SharedFx)
                {
                    commands.Add(new InstallCommand<SharedFxAsset>(fx));
                }

                foreach (var tool in context.ConfigFile.Tools)
                {
                    commands.Add(new InstallToolCommand(tool.Key, tool.Value));
                }

                if (commands.Count == 0)
                {
                    context.Reporter.Warn("Nothing will be installed because the config file does not list assets.");
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