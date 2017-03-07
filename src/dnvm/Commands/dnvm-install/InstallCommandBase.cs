using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DotNet.VersionManager.Assets;
using DotNet.VersionManager.IO;

namespace DotNet.VersionManager.Commands
{
    public abstract class InstallCommandBase : ICommand
    {
        public virtual async Task ExecuteAsync(CommandContext context)
        {
            Directory.CreateDirectory(context.Environment.Root);

            var dependencies = new HashSet<Asset>();
            foreach (var asset in CreateAssets(context))
            {
                CollectDependencies(asset, dependencies);
            }

            if (dependencies.Count == 0)
            {
                context.Logger.Warn("Nothing will be installed because the config file does not list assets.");
            }
            else
            {
                context.Logger.Output("Installing:");
                foreach (var dep in dependencies.OrderBy(f => f.DisplayName))
                {
                    context.Logger.Output($"  {dep.DisplayName}");
                }
                context.Logger.Output("");
            }

            var success = false;
            try
            {
                foreach (var asset in dependencies)
                {
                    if (asset.IsInstalled)
                    {
                        context.Logger.Output(ConsoleEventIds.DarkGray, $"Skipping {asset.DisplayName}. Already installed.");
                        continue;
                    }

                    if (!await asset.InstallAsync(context.CancellationToken))
                    {
                        success = false;
                        break;
                    }
                }

                success = true;
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

            context.Logger.Output("");
            context.Logger.Output(ConsoleEventIds.Green, "Done");

            context.Result = Result.Okay;
        }

        protected abstract IEnumerable<Asset> CreateAssets(CommandContext context);

        private void CollectDependencies(Asset asset, HashSet<Asset> deps)
        {
            if (deps.Contains(asset))
            {
                return;
            }

            deps.Add(asset);

            foreach (var dep in asset.Dependencies)
            {
                CollectDependencies(dep, deps);
            }
        }
    }
}
