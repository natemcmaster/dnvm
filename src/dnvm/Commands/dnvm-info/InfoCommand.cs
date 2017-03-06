using System.Linq;

namespace DotNet.VersionManager.Commands
{
    class InfoCommand : SyncCommand
    {
        private const string None = "<none>";

        protected override void Execute(CommandContext context)
        {
            context.Logger.Output($"Name     : {context.Environment.Name}");
            context.Logger.Output($"Location : {context.Environment.Root}");
            context.Logger.Output($"Config   : {context.ConfigFile?.FilePath ?? None}");
            context.Logger.Output("");
            context.Logger.Output(".NET Core SDK");
            context.Logger.Output("-------------");

            if (!context.Environment.Sdks.Any())
            {
                context.Logger.Output("    " + None);
            }

            foreach (var sdk in context.Environment.Sdks)
            {
                context.Logger.Output("    " + sdk.Version);
            }

            context.Logger.Output("");

            context.Logger.Output("Shared Frameworks");
            context.Logger.Output("-----------------");

            if (!context.Environment.Frameworks.Any())
            {
                context.Logger.Output("    " + None);
            }

            foreach (var fx in context.Environment.Frameworks)
            {
                context.Logger.Output($"{fx.Name}@{fx.Version}");
            }

            context.Logger.Output("");

            context.Logger.Output("Command-line tools");
            context.Logger.Output("------------------");

            if (!context.Environment.Tools.Any())
            {
                context.Logger.Output("    " + None);
            }

            foreach (var tool in context.Environment.Tools)
            {
                context.Logger.Output($"    {tool.Name}@{tool.Version}");
            }

            context.Result = Result.Okay;
        }
    }
}
