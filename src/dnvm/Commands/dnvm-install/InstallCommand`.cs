using System.IO;
using System.Threading.Tasks;
using DotNet.Assets;

namespace DotNet.Commands
{
    public abstract class InstallCommand<TAsset> : ICommand
        where TAsset : Asset
    {
        public async Task ExecuteAsync(CommandContext context)
        {
            var asset = CreateAsset(context);
            Directory.CreateDirectory(context.Environment.Root);

            if (!await asset.InstallAsync(context.CancellationToken))
            {
                context.Result = Result.Error;
                return;
            }

            context.Result = Result.Okay;
        }

        protected abstract TAsset CreateAsset(CommandContext context);
    }
}