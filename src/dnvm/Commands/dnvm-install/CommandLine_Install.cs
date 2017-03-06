using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;
using DotNet.VersionManager.Files;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    partial class CommandLine
    {
        private void InstallCommand(CommandLineApplication c)
        {
            c.Command("fx", "Install a .NET Core runtime framework", InstallFxCommand);

            c.Command("sdk", "Install a .NET Core SDK", InstallSdkCommand);

            c.Command("tool", "Install a .NET Core CLI tool", InstallToolCommand);

            c.OnExecute(() =>
            {
                this.Command = new InstallFromFileCommand();
            });

            var dnvm = FileConstants.Config;

            // TODO add dnvm-config with help about the file structure of the .dnvm file
            c.ExtendedHelpText = $@"
Additional Information:
  If executed without arguments, the 'install' command will search for the
  dnvm config file (named '{dnvm}') and attempt to install any assets
  specified in the file.
";
        }

        private void InstallFxCommand(CommandLineApplication fx)
        {
            var argVersion = fx.Argument("version",
                    $"Version of the shared framework to install. Defaults to '{SharedFxAsset.DefaultVersion}'.");

/*
            TODO support architecture-specific assets
            var optArch = fx.Option("-a|--arch",
                    $"Processor architecture of the framework. Accepted values: x86, x64. Defaults to {defaultArch}");
*/

            var optSave = fx.Option("--save",
                $"Save to the 'fx' version of the '{FileConstants.Config}' config file.", CommandOptionType.NoValue);

            fx.OnExecute(() =>
            {
                var version = argVersion.Value ?? SharedFxAsset.DefaultVersion;

                ICommand install = new InstallFxCommand(version, Architecture.X64);

                if (optSave.HasValue())
                {
                    install = CommonCommands.Sequence(
                        CommonCommands.EnsureConfigFileExists,
                        install,
                        new EditConfigCommand(c => c.SharedFx.Add(version)));
                }

                this.Command = install;
            });
        }

        private void InstallSdkCommand(CommandLineApplication sdk)
        {
            var argVersion = sdk.Argument("version", $"Version of the .NET Core SDK to install. Defaults to '{SdkAsset.DefaultVersion}'.");
            var optSave = sdk.Option("--save", $"Save as the value of 'sdk' in the '{FileConstants.Config}' config file.", CommandOptionType.NoValue);

            sdk.OnExecute(() =>
            {
                var version = argVersion.Value ?? SdkAsset.DefaultVersion;
                ICommand install = new InstallSdkCommand(version, Architecture.X64);
                if (optSave.HasValue())
                {
                    install = CommonCommands.Sequence(
                        CommonCommands.EnsureConfigFileExists,
                        install,
                        new EditConfigCommand(c => c.Sdk = version));
                }

                this.Command = install;
            });
        }

        private void InstallToolCommand(CommandLineApplication tool)
        {
            var argName = tool.Argument("name", $"Name of the tool. Required.");
            var argVersion = tool.Argument("version", $"The version of the tool. Defaults to '{ToolAsset.DefaultVersion}'.");
            var optSave = tool.Option(
                "--save", $"Save the tool to the 'tools' in the '{FileConstants.Config}' config file.",
                CommandOptionType.NoValue);

            tool.OnExecute(() =>
            {
                var name = argName.IfNotNullOrEmpty();
                var version = argVersion.Value ?? ToolAsset.DefaultVersion;
                ICommand install = new InstallToolCommand(name, version);
                if (optSave.HasValue())
                {
                    install = CommonCommands.Sequence(
                        CommonCommands.EnsureConfigFileExists,
                        install,
                        new EditConfigCommand(c => c.Tools[name.ToLowerInvariant()] = version));
                }

                this.Command = install;
            });
        }
    }
}
