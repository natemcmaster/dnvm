using DotNet.Commands;

namespace DotNet.Reporting
{
    static class ReporterExtensions
    {
        public static void Output(this IReporter reporter)
        {
            reporter.Output(string.Empty);
        }

        public static IndentReportingScope Indent(this CommandContext context)
        {
            return new IndentReportingScope(context, spaces: 4);
        }
    }
}