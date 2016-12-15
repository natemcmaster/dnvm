using DotNet.Packaging;

namespace DotNet.Commands
{
    class InstallCommand : ICommand
    {

        public InstallCommand()
        {
        }

        public void Execute(CommandContext context)
        {
            new Downloader(context.Reporter).FetchAsync().GetAwaiter().GetResult();
            context.Result = Result.Done;
        }
    }
}