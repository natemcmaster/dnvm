using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    partial class CommandLine
    {
        private void SearchCommand(CommandLineApplication c)
        {
            c.Command("fx", "Search available versions of .NET Core runtime frameworks", fx =>
            {
                fx.OnExecute(() =>
                {
                    this.Command = new SearchFxCommand();
                });
            });

            c.Command("sdk", "Search available versions of .NET Core SDKs", sdk =>
            {
                sdk.OnExecute(() =>
                {
                    this.Command = new SearchSdkCommand();
                });
            });

            c.OnExecute(() =>
            {
                c.ShowHelp();
            });
        }
    }
}
