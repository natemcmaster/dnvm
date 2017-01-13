using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Files;
using DotNet.Reporting;
using DotNet.Utils;

namespace DotNet.Assets
{
    public class ToolAsset : AssetBase
    {
        public const string DefaultVersion = "latest";
        private readonly IAssetChannel _channel = new ToolAssetChannel();

        private readonly string _assetId;
        private readonly string _version;
        private readonly DotNetEnv _env;
        public ToolAsset(IReporter reporter, DotNetEnv env, string name, string version)
            : base(reporter)
        {
            _assetId = $"dnvm.tool.{name}";
            _version = version == DefaultVersion
                ? _channel.GetLatestVersion(_assetId)
                : version;

            _env = env;
            DisplayName = $"dotnet-{name} {_version}";
        }

        public override string DisplayName { get; }

        public override bool Uninstall()
        {
            Reporter.Output($"Removing {DisplayName}");
            var dest = GetToolRoot();
            if (Directory.Exists(dest))
            {
                UnlinkExecutables();
            }

            return UninstallFolder(dest);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Reporter.Output($"Installing {DisplayName}");
            var dest = GetToolRoot();

            if (Directory.Exists(dest))
            {
                Reporter.Verbose("Skipping. Already installed");
                // TODO ensure commands are linkied
                return true;
            }

            var url = _channel.GetDownloadUrl(_assetId, _version);
            if (!await DownloadAndExtractAsync(url, dest, ZipExtractor.Extract, cancellationToken))
            {
                return false;
            }

            var manifestFile = Path.Combine(dest, "dnvm.json");
            if (!File.Exists(manifestFile))
            {
                Reporter.Error("This tool is missing the 'dnvm.json' manifest file");
                Uninstall();
                return false;
            }

            try
            {
                var manifest = ToolManifest.LoadFromFile(manifestFile);
                var secondaryInstalls = new List<Task>();
                foreach (var command in manifest.Commands)
                {
                    if (!LinkExecutable(command.Key, command.Value))
                    {
                        Uninstall();
                        return false;
                    }

                    if (command.Value.Portable == true)
                    {
                        var runtimeConfigPath = Path.Combine(dest, command.Value.RuntimeConfig);
                        var runtimeConfig = RuntimeConfig.LoadFromFile(runtimeConfigPath);
                        var name = runtimeConfig?.RuntimeOptions?.Framework?.Name;

                        if (!SharedFxAsset.AssetId.Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            Reporter.Error($"This tool requires an unsupported shared framework: {name}");
                            Uninstall();
                            return false;
                        }

                        var version = runtimeConfig.RuntimeOptions.Framework.Version;
                        var sharedFx = new SharedFxAsset(Reporter, _env, version);
                        secondaryInstalls.Add(sharedFx.InstallAsync(cancellationToken));
                    }
                }

                await Task.WhenAll(secondaryInstalls);
            }
            catch (FormatException ex)
            {
                Reporter.Verbose(ex.Message);
                Reporter.Error("This tool has an unrecognized 'dnvm.json' manifest format");
                Uninstall();
                return false;
            }

            return true;
        }

        private string GetToolRoot()
            => Path.Combine(_env.ToolsRoot, _assetId.ToLowerInvariant(), _version);

        private string GetToolExecutable(string name)
        {
            var target = $"dotnet-{name}";
            return Path.Combine(_env.BinRoot, target);
        }

        private bool LinkExecutable(string name, ToolManifest.Command command)
        {
            // TODO keep track of created files for the uninstaller to also remove
            var targetPath = GetToolExecutable(name);

            if (File.Exists(targetPath))
            {
                Reporter.Verbose($"File already exists: {targetPath}");
                Reporter.Error($"A tool with a command named {name} has already been installed");
                return false;
            }

            Reporter.Verbose($"Creating tool executable in '{targetPath}'");

            if (command.Portable)
            {
                var exe = Path.Combine(_assetId.ToLowerInvariant(), _version, command.Exe);
                Directory.CreateDirectory(_env.BinRoot);
                File.WriteAllText(targetPath, $@"#!/usr/bin/env bash
set -e

# resolve $SOURCE until the file is no longer a symlink
SOURCE=""${{BASH_SOURCE[0]}}""
while [ -h ""$SOURCE"" ]; do
  DIR=""$( cd -P ""$( dirname ""$SOURCE"" )"" && pwd )""
  SOURCE=""$(readlink ""$SOURCE"")""
  [[ ""$SOURCE"" != /* ]] && SOURCE=""$DIR/$SOURCE""
done
DIR=""$( cd -P ""$( dirname ""$SOURCE"" )"" && pwd )""

""$DIR/../dotnet"" ""$DIR/../tools/{exe}"" ""$@""
");

                var psi = new ProcessStartInfo
                {
                    FileName = "chmod",
                    Arguments = ArgumentEscaper.EscapeAndConcatenate(new[] { "+x", targetPath }),
                    UseShellExecute = false,
                };

                Reporter.Debug($"Executing {psi.FileName} {psi.Arguments}");

                var chmod = Process.Start(psi);
                chmod.WaitForExit();
                if (chmod.ExitCode != 0)
                {
                    Reporter.Warn($"Failed to make '{targetPath}' executable. Please run 'chmod +x {targetPath}'");
                }
            }
            else
            {
                throw new NotImplementedException("Non-portable tools not yet supported");
            }

            return true;
        }

        private void UnlinkExecutables()
        {
            var dest = GetToolRoot();
            var manifestFile = Path.Combine(dest, "dnvm.json");
            try
            {
                var manifest = ToolManifest.LoadFromFile(manifestFile);
                foreach (var command in manifest.Commands)
                {
                    var exe = GetToolExecutable(command.Key);
                    if (File.Exists(exe))
                    {
                        Reporter.Verbose($"Deleting '{exe}'");
                        File.Delete(exe);
                    }
                }
            }
            catch
            {
                Reporter.Verbose("Unable to read tool manifest.");
            }
        }
    }
}