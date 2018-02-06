using System.Threading;
using DotNet.VersionManager.Files;
using DotNet.VersionManager.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;

namespace DotNet.VersionManager.Commands
{
    public class CommandContext
    {
        public CancellationToken CancellationToken { get; set; }
        public Result Result { get; set; }
        public DotNetEnv Environment { get; set; }

        public ConfigFile ConfigFile { get; set; }

        public IConsole Console { get; set; }
        public string WorkingDir { get; set; }

        public ILogger Logger { get; set; }
        public DnvmSettings Settings { get; set; }
    }
}
