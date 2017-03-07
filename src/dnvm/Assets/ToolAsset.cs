using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DotNet.VersionManager.Files;
using DotNet.VersionManager.Utils;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DotNet.VersionManager.Assets
{
    public class ToolAsset : AssetBase
    {
        public const string DefaultVersion = "stable";
        private readonly IAssetChannel _channel = new ToolAssetChannel();
        private readonly AssetInfo _assetInfo;
        private readonly DotNetEnv _env;
        public ToolAsset(ILogger logger, DotNetEnv env, string name, string version)
            : base(logger)
        {
            _env = env;

            var id = $"dnvm.tool.{name}";
            _assetInfo = version == DefaultVersion
                ? _channel.GetLatest(id)
                : _channel.GetAssetInfo(id, version);

            DisplayName = $"dotnet-{name} {_assetInfo.Version}";
        }

        public override string DisplayName { get; }

        public override IEnumerable<Asset> Dependencies
            => Enumerable.Empty<Asset>();

        public override bool IsInstalled
            => Directory.Exists(GetToolRoot());

        public override bool Uninstall()
        {
            Log.Output($"Uninstalling {DisplayName}");
            var dest = GetToolRoot();
            if (Directory.Exists(dest))
            {
                UnlinkExecutables();
            }

            return UninstallFolder(dest);
        }

        public override async Task<bool> InstallAsync(CancellationToken cancellationToken)
        {
            Log.Output($"Installing {DisplayName}");
            var dest = GetToolRoot();

            var url = _assetInfo.DownloadUrl;
            if (!await DownloadAndExtractAsync(url, dest, ZipExtractor.Extract, cancellationToken))
            {
                return false;
            }

            var manifestFile = Path.Combine(dest, "dnvm.json");
            if (!File.Exists(manifestFile))
            {
                Log.Error("This tool is missing the 'dnvm.json' manifest file");
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

                        if (!"Microsoft.NETCore.App".Equals(name, StringComparison.OrdinalIgnoreCase))
                        {
                            Log.Error($"This tool requires an unsupported shared framework: {name}");
                            Uninstall();
                            return false;
                        }

                        var version = runtimeConfig.RuntimeOptions.Framework.Version;
                        var sharedFx = new RuntimeAsset(Log, _env, version, Architecture.X64);
                        secondaryInstalls.Add(sharedFx.InstallAsync(cancellationToken));
                    }
                }

                await Task.WhenAll(secondaryInstalls);
            }
            catch (FormatException ex)
            {
                Log.Verbose(ex.Message);
                Log.Error("This tool has an unrecognized 'dnvm.json' manifest format");
                Uninstall();
                return false;
            }

            Log.Output($"Installed {DisplayName}");

            return true;
        }

        private string GetToolRoot()
            => Path.Combine(_env.ToolsRoot, _assetInfo.Id.ToLowerInvariant(), _assetInfo.Version);

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
                Log.Verbose($"File already exists: {targetPath}");
                Log.Error($"A tool with a command named {name} has already been installed");
                return false;
            }

            Log.Verbose($"Creating tool executable in '{targetPath}'");

            if (command.Portable)
            {
                var exe = Path.Combine(_assetInfo.Id.ToLowerInvariant(), _assetInfo.Version, command.Exe);
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

                Log.Trace($"Executing {psi.FileName} {psi.Arguments}");

                var chmod = Process.Start(psi);
                chmod.WaitForExit();
                if (chmod.ExitCode != 0)
                {
                    Log.Warn($"Failed to make '{targetPath}' executable. Please run 'chmod +x {targetPath}'");
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
                        Log.Verbose($"Deleting '{exe}'");
                        File.Delete(exe);
                    }
                }
            }
            catch
            {
                Log.Verbose("Unable to read tool manifest.");
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            var other = (ToolAsset)obj;
            return _assetInfo.Id == other._assetInfo.Id
                && _assetInfo.Version == other._assetInfo.Version
                && _env.Name == other._env.Name;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + _assetInfo.Id.GetHashCode();
                hash = hash * 23 + _assetInfo.Version.GetHashCode();
                hash = hash * 23 + _env.Name.GetHashCode();
                return hash;
            }
        }
    }
}
