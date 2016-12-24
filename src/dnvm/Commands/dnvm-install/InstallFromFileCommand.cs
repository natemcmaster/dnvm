using System;
using System.Threading.Tasks;

namespace DotNet.Commands
{
    class InstallFromFileCommand : ICommand
    {
        public Task ExecuteAsync(CommandContext context)
        {
            // should check the .dnvm file and install any missing assets
            throw new NotImplementedException();
        }
    }
}