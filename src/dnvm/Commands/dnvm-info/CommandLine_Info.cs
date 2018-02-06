using McMaster.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    partial class CommandLine
    {
        private void Info(CommandLineApplication c)
        {
            c.OnExecute(() =>
            {
                this.Command = new InfoCommand();
            });
        }
    }
}
