using DotNet.Files;
using DotNet.Reporting;

namespace DotNet.Commands
{
    class CommandContext
    {
        public Result Result { get; set; }
        public Environment Environment { get; set; }
        public IReporter Reporter { get; set; }
        public DnvmSettings Settings { get; set; }
    }
}