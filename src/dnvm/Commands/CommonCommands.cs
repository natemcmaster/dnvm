using System.Collections.Generic;

namespace DotNet.Commands
{
    public class CommonCommands
    {
        public static ICommand Sequence(IEnumerable<ICommand> commands)
            => new CompositeCommand(commands);

        public static ICommand Sequence(params ICommand[] commands)
            => new CompositeCommand(commands);

        public static readonly ICommand EnsureConfigFileExists = new EnsureConfigCommand();

        private class EnsureConfigCommand : SyncCommand
        {
            protected override void Execute(CommandContext context)
            {
                if (context.ConfigFile == null)
                {
                    context.Result = Result.Error;
                    context.Reporter.Error("Could not find a config file.");
                    context.Reporter.Output("Try executing `dnvm init` to create a config file.");
                    return;
                }
            }
        }
    }
}