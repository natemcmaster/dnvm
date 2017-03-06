using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    partial class CommandLine
    {
        private void InitCommand(CommandLineApplication c)
        {
            c.OnExecute(() =>
            {
                this.Command = new InitConfigInteractiveCommand();
            });
        }
    }
}
