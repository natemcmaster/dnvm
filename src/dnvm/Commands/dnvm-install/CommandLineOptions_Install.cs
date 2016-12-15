using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLineOptions
    {
        private void InstallCommand()
        {
            _syntax.Command("install", "Install things", c =>
            {
                c.Command("fx", "Install a shared framework", fx =>
                {

                });
                c.Command("sdk", "Install a .NET Core SDK", sdk =>
                {

                });

                c.OnExecute(() =>
                {
                    this.Command = new InstallCommand();
                });
            });
        }
    }
}