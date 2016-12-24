using DotNet.Assets;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLineOptions
    {
        private void InstallCommand()
        {
            _syntax.Command("install", "Install things", c =>
            {
                c.Command("fx", "Install a shared framework", ConfigureFxCommand);

                c.Command("cli", "Install a .NET Core CLI", ConfigureCliCommand);

                c.OnExecute(() =>
                {
                    this.Command = new InstallFromFileCommand();
                });

                c.ExtendedHelpText = $@"
Additional Information:
  If executed without arguments, the 'install' command will search for the 
  dnvm config file (named '{Files.FileConstants.Config}') and attempt to install 
  any assets specified in the file.
";
            });
        }

        private void ConfigureFxCommand(CommandLineApplication fx)
        {
            var argVersion = fx.Argument("version",
                    $"Version of the shared framework to install. Defaults to '{SharedFxAsset.DefaultVersion}'");

            fx.OnExecute(() =>
            {
                this.Command = new InstallCommand(SharedFxAsset.Name, argVersion.Value ?? SharedFxAsset.DefaultVersion);
            });
        }

        private void ConfigureCliCommand(CommandLineApplication cli)
        {
            var argVersion = cli.Argument("version", $"Version of the CLI install. Defaults to '{CliAsset.DefaultVersion}'");

            cli.OnExecute(() =>
            {
                this.Command = new InstallCommand(CliAsset.Name, argVersion.Value ?? CliAsset.DefaultVersion);
            });
        }
    }
}