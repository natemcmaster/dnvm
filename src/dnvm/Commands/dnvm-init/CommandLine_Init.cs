using McMaster.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    partial class CommandLine
    {
        private void Init(CommandLineApplication c)
        {
            c.OnExecute(() =>
            {
                this.Command = new InitConfigInteractiveCommand();
            });
        }
    }
}
