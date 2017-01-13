using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLine
    {
        private void InfoCommand(CommandLineApplication c)
        {
            c.OnExecute(() =>
            {
                this.Command = new InfoCommand();
            });
        }
    }
}