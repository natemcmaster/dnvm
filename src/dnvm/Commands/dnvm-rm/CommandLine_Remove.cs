using Microsoft.Extensions.CommandLineUtils;
using System.Runtime.InteropServices;

namespace DotNet.Commands
{
    partial class CommandLine
    {
        // TODO implement --save
        private void RemoveCommand(CommandLineApplication c)
        {
            c.Command("fx", "Remove a .NET Core runtime framework", fx =>
            {
                var version = fx.Argument("version", "The version of the framework to remove");
                var force = fx.Option("-f|--force", "Uninstall without asking questions",
                        CommandOptionType.NoValue);

                fx.OnExecute(() =>
                {
                    this.Command = new RemoveFxCommand(version.IfNotNullOrEmpty(), Architecture.X64, force.HasValue());
                });
            });

            c.Command("sdk", "Remove a .NET Core SDK", sdk =>
            {
                var version = sdk.Argument("version", "The version of the SDK to remove");
                var force = sdk.Option("-f|--force", "Uninstall without asking questions",
                        CommandOptionType.NoValue);

                sdk.OnExecute(() =>
                {
                    this.Command = new RemoveSdkCommand(version.IfNotNullOrEmpty(), Architecture.X64, force.HasValue());
                });
            });

            c.Command("tool", "Remove a .NET Core tool", tool =>
           {
               var name = tool.Argument("name", "The name of tool to remove");
               var version = tool.Argument("version", "The version of tool to remove");
               var force = tool.Option("-f|--force", "Uninstall without asking questions",
                       CommandOptionType.NoValue);

               tool.OnExecute(() =>
               {
                   this.Command = new RemoveToolCommand(
                       name.IfNotNullOrEmpty(),
                       version.IfNotNullOrEmpty(),
                       force.HasValue());
               });
           });

            c.OnExecute(() =>
            {
                c.ShowHelp();
            });
        }
    }
}