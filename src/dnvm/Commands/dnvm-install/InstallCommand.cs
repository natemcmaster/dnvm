using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Assets;
using DotNet.Reporting;

namespace DotNet.Commands
{
    public class InstallCommand : ICommand
    {
        private readonly string _assetName;
        private readonly string _version;

        public InstallCommand(string assetName, string version)
        {
            _assetName = assetName;
            _version = version;
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var asset = CreateAsset(context);
            Directory.CreateDirectory(context.Environment.Root);

            await asset.InstallAsync(context.CancellationToken);

            context.Reporter.Output(ColorFormatter.Format("Done", ConsoleColor.Green));
            context.Result = Result.Done;
        }

        private Asset CreateAsset(CommandContext context)
        {
            if (_assetName.Equals(SharedFxAsset.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new SharedFxAsset(context.Reporter, context.Environment, _version);
            }
            else if(_assetName.Equals(CliAsset.Name, StringComparison.OrdinalIgnoreCase))
            {
                return new CliAsset(context.Reporter, context.Environment, _version);
            }

            throw new InvalidOperationException("Unrecognized asset name: " + _assetName);
        }
    }
}