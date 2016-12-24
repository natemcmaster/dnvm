using System.Threading.Tasks;

namespace DotNet.Commands
{
    public interface ICommand
    {
        Task ExecuteAsync(CommandContext context);
    }
}