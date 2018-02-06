using System.Collections.Generic;
using System.Linq;
using DotNet.VersionManager.Assets;
using DotNet.VersionManager.IO;
using McMaster.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
{
    public abstract class RemoveCommandBase : SyncCommand
    {
        private readonly bool _force;

        public RemoveCommandBase(bool force)
        {
            _force = force;
        }

        protected override void Execute(CommandContext context)
        {
            var assets = CreateAssets(context).Where(a => a.IsInstalled);

            if (!assets.Any())
            {
                context.Result = Result.Okay;
                context.Logger.Output("Nothing found to uninstall");
                return;
            }

            context.Logger.Output(_force ? "Removing:" : "This will remove:");

            foreach (var asset in assets.OrderBy(f => f.DisplayName))
            {
                context.Logger.Output($"  {asset.DisplayName}");
            }

            context.Logger.Output("");

            if (!_force)
            {
                var answer = Prompt.GetYesNo($"Are you sure you want to proceed?", defaultAnswer: false);
                if (!answer)
                {
                    context.Logger.Output("Cancelled");
                    context.Result = Result.Okay;
                    return;
                }
            }

            var success = true;
            foreach (var asset in assets)
            {
                success &= asset.Uninstall();
            }

            if (success)
            {
                context.Logger.Output("");
                context.Logger.Output(ConsoleEventIds.Green, "Done");
            }

            context.Result = success
                ? Result.Okay
                : Result.Error;
        }

        protected abstract IEnumerable<Asset> CreateAssets(CommandContext context);
    }
}
