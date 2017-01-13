using DotNet.Assets;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLine
    {
        private void RemoveCommand(CommandLineApplication c)
        {
            c.Command("fx", "Remove a .NET Core runtime framework", fx =>
            {
                var version = fx.Argument("version", "The version of the framework to remove");
                var force = fx.Option("-f|--force", "Uninstall without asking questions",
                        CommandOptionType.NoValue);

                fx.OnExecute(() =>
                {
                    this.Command = new RemoveCommand<SharedFxAsset>(version.IfNotNullOrEmpty(), force.HasValue());
                });
            });

            c.Command("sdk", "Remove a .NET Core SDK", sdk =>
            {
                var version = sdk.Argument("version", "The version of the SDK to remove");
                var force = sdk.Option("-f|--force", "Uninstall without asking questions",
                        CommandOptionType.NoValue);

                sdk.OnExecute(() =>
                {
                    this.Command = new RemoveCommand<SdkAsset>(version.IfNotNullOrEmpty(), force.HasValue());
                });
            });

            c.OnExecute(() =>
            {
                c.ShowHelp();
            });
        }
    }
}