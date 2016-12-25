using System.Linq;
using System.Threading.Tasks;
using DotNet.Reporting;

namespace DotNet.Commands
{
    class InfoCommand : ICommand
    {
        public Task ExecuteAsync(CommandContext context)
        {
            context.Reporter.Output($"Name     : {context.Environment.Name}");
            context.Reporter.Output($"Location : {context.Environment.Root}");
            context.Reporter.Output();
            context.Reporter.Output(".NET Core SDK");
            context.Reporter.Output("-------------");

            if (!context.Environment.Clis.Any())
            {
                using (context.Indent())
                {
                    context.Reporter.Output("<none>");
                }
            }

            foreach (var sdk in context.Environment.Clis)
            {
                using (context.Indent())
                {
                    context.Reporter.Output(sdk.Version);
                }
            }

            context.Reporter.Output();

            context.Reporter.Output("Shared Frameworks");
            context.Reporter.Output("-----------------");

            foreach (var fx in context.Environment.Frameworks)
            {
                using (context.Indent())
                {
                    context.Reporter.Output($"{fx.Name}/{fx.Version}");
                }
            }

            context.Result = Result.Done;
            return Task.CompletedTask;
        }
    }
}