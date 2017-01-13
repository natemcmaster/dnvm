using System.Collections.Generic;
using System.Threading.Tasks;

namespace DotNet.Commands
{
    public class CompositeCommand : ICommand
    {
        private readonly IEnumerable<ICommand> _commands;

        public CompositeCommand(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            foreach (var cmd in _commands)
            {
                await cmd.ExecuteAsync(context);
                if (context.Result == Result.Error)
                {
                    break;
                }
            }
        }
    }
}