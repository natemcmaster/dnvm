using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public abstract class RemoveCommand<TAsset> : SyncCommand
        where TAsset : Asset
    {
        private readonly bool _force;

        public RemoveCommand(bool force)
        {
            _force = force;
        }

        protected override void Execute(CommandContext context)
        {
            var asset = CreateAsset(context);

            if (!_force)
            {
                var answer = context.Console.Prompt($"Are you sure you want to remove {asset.DisplayName}? (y/N) ");
                if (!"y".Equals(answer?.Trim()?.ToLowerInvariant()))
                {
                    context.Logger.Output("Cancelled");
                    context.Result = Result.Okay;
                    return;
                }
            }

            context.Result = asset.Uninstall()
                ? Result.Okay
                : Result.Error;
        }

        protected abstract TAsset CreateAsset(CommandContext context);
    }
}
