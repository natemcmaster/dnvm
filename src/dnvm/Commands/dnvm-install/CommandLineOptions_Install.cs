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
                c.Command("fx", "Install a .NET Core runtime framework", ConfigureFxCommand);

                c.Command("sdk", "Install a .NET Core SDK", ConfigureCliCommand);

                c.OnExecute(() =>
                {
                    this.Command = new InstallFromFileCommand();
                });

                var dnvm = Files.FileConstants.Config;
                c.ExtendedHelpText = $@"
Additional Information:
  If executed without arguments, the 'install' command will search for the 
  dnvm config file (named '{dnvm}') and attempt to install any assets 
  specified in the file.
"; // TODO add dnvm-config with help about the file structure of the .dnvm file
            });
        }

        private void ConfigureFxCommand(CommandLineApplication fx)
        {
            var argVersion = fx.Argument("version",
                    $"Version of the shared framework to install. Defaults to '{SharedFxAsset.DefaultVersion}'");

            fx.OnExecute(() =>
            {
                this.Command = new InstallCommand<SharedFxAsset>(argVersion.Value ?? SharedFxAsset.DefaultVersion);
            });
        }

        private void ConfigureCliCommand(CommandLineApplication sdk)
        {
            var argVersion = sdk.Argument("version", $"Version of the CLI install. Defaults to '{SdkAsset.DefaultVersion}'");

            sdk.OnExecute(() =>
            {
                this.Command = new InstallCommand<SdkAsset>(argVersion.Value ?? SdkAsset.DefaultVersion);
            });
        }
    }
}