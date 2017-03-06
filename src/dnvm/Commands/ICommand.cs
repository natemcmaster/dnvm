using System.Threading.Tasks;

namespace DotNet.VersionManager.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(CommandContext context);
    }
}
