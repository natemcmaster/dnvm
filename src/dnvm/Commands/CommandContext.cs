using System.Threading;
using DotNet.Files;
using DotNet.Reporting;

namespace DotNet.Commands
{
    public class CommandContext
    {
        public CancellationToken CancellationToken { get; set; }
        public Result Result { get; set; }
        public DotNetEnv Environment { get; set; }
        public IReporter Reporter { get; set; }
        public DnvmSettings Settings { get; set; }
    }
}