using System;
using System.IO;
using System.Threading.Tasks;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public abstract class InstallCommand<TAsset> : ICommand
        where TAsset : Asset
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            var asset = CreateAsset(context);
            Directory.CreateDirectory(context.Environment.Root);

            bool success;
            try
            {
                success = await asset.InstallAsync(context.CancellationToken);
            }
            catch (Exception ex)
            {
                context.Logger.Verbose(ex.ToString());
                context.Logger.Error(ex.Message);
                success = false;
            }

            if (!success)
            {
                context.Result = Result.Error;
                return;
            }

            context.Result = Result.Okay;
        }

        protected abstract TAsset CreateAsset(CommandContext context);
    }
}
