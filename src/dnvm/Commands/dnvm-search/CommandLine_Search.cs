using McMaster.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    partial class CommandLine
    {
        private void Search(CommandLineApplication c)
        {
            c.Command("runtime", "Search available .NET Core runtimes", runtime =>
            {
                runtime.OnExecute(() =>
                {
                    this.Command = new SearchRuntimeCommand();
                });
            });

            c.Command("sdk", "Search available .NET Core SDKs", sdk =>
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
