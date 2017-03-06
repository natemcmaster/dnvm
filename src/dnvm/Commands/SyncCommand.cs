using System.Threading.Tasks;

namespace DotNet.VersionManager.Commands
{
    public abstract class SyncCommand : ICommand
    {
        public Task ExecuteAsync(CommandContext context)
        {
            Execute(context);
            return Task.CompletedTask;
        }

        protected abstract void Execute(CommandContext context);
    }
}
