using System.Linq;
using DotNet.Reporting;

namespace DotNet.Commands
{
    class InfoCommand : SyncCommand
    {
        private const string None = "<none>";

        protected override void Execute(CommandContext context)
        {
            context.Reporter.Output($"Name     : {context.Environment.Name}");
            context.Reporter.Output($"Location : {context.Environment.Root}");
            context.Reporter.Output($"Config   : {context.ConfigFile?.FilePath ?? None}");
            context.Reporter.Output();
            context.Reporter.Output(".NET Core SDK");
            context.Reporter.Output("-------------");

            if (!context.Environment.Sdks.Any())
            {
                using (context.Indent())
                {
                    context.Reporter.Output(None);
                }
            }

            foreach (var sdk in context.Environment.Sdks)
            {
                using (context.Indent())
                {
                    context.Reporter.Output(sdk.Version);
                }
            }

            context.Reporter.Output();

            context.Reporter.Output("Shared Frameworks");
            context.Reporter.Output("-----------------");

            if (!context.Environment.Frameworks.Any())
            {
                using (context.Indent())
                {
                    context.Reporter.Output(None);
                }
            }

            foreach (var fx in context.Environment.Frameworks)
            {
                using (context.Indent())
                {
                    context.Reporter.Output($"{fx.Name}@{fx.Version}");
                }
            }

            context.Reporter.Output();

            context.Reporter.Output("Command-line tools");
            context.Reporter.Output("------------------");

            if (!context.Environment.Tools.Any())
            {
                using (context.Indent())
                {
                    context.Reporter.Output(None);
                }
            }

            foreach (var tool in context.Environment.Tools)
            {
                using (context.Indent())
                {
                    context.Reporter.Output($"{tool.Name}@{tool.Version}");
                }
            }

            context.Result = Result.Okay;
        }
    }
}