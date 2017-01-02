using DotNet.Files;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLineOptions
    {
        private void InitCommand(CommandLineApplication app)
        {
            app.Command("init", init =>
            {
                init.Description = $"Initializes a new '{FileConstants.Config}' config file in the current directory.";
                init.OnExecute(() =>
                {
                    this.Command = new InitConfigInteractiveCommand();
                });
            });
        }
    }
}