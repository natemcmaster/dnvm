using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLine
    {
        private void ListCommand(CommandLineApplication c)
        {
            c.Command("fx", "Show available versions of .NET Core runtime frameworks", fx =>
            {
                fx.OnExecute(() =>
                {
                    this.Command = new ListFxCommand();
                });
            });

            c.Command("sdk", "Show available versions of .NET Core SDKs", sdk =>
            {
                sdk.OnExecute(() =>
                {
                    this.Command = new ListSdkCommand();
                });
            });

            c.OnExecute(() =>
            {
                c.ShowHelp();
            });
        }
    }
}