using System.Threading.Tasks;

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
                
            }
        }
    }
}