using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.Assets;
using DotNet.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Commands
{
    public class InstallCommand<TAsset> : ICommand
        where TAsset : Asset
    {
        private readonly string _version;

        public InstallCommand(string version)
        {
            _version = version;
        }

        public async Task ExecuteAsync(CommandContext context)
        {
            var asset = context.Services.GetRequiredService<IAssetFactory>().Create<TAsset>(_version);
            Directory.CreateDirectory(context.Environment.Root);

            if (!await asset.InstallAsync(context.CancellationToken))
            {
                context.Result = Result.Error;
                return;
            }

            context.Reporter.Output(ColorFormatter.Format("Done", ConsoleColor.Green));
            context.Result = Result.Done;
        }
    }
}