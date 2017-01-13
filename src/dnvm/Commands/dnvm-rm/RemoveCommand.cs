using DotNet.Assets;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Commands
{
    public class RemoveCommand<TAsset> : SyncCommand
        where TAsset : Asset
    {
        private readonly string _version;
        private readonly bool _force;

        public RemoveCommand(string version, bool force)
        {
            _version = version;
            _force = force;
        }

        protected override void Execute(CommandContext context)
        {
            var asset = context.Services.GetRequiredService<IAssetFactory>().Create<TAsset>(_version);

            if (!_force)
            {
                var answer = context.Console.Prompt($"Are you sure you want to remove {asset.DisplayName}? (y/N) ");
                if (!"y".Equals(answer?.Trim()?.ToLowerInvariant()))
                {
                    context.Reporter.Output("Cancelled");
                    context.Result = Result.Okay;
                    return;
                }
            }

            context.Result = asset.Uninstall()
                ? Result.Okay
                : Result.Error;
        }
    }
}