using System;
using System.Threading;
using DotNet.Files;
using DotNet.Reporting;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet.Commands
{
    public class CommandContext
    {
        public CancellationToken CancellationToken { get; set; }
        public Result Result { get; set; }
        public DotNetEnv Environment { get; set; }

        public ConfigFile ConfigFile { get; set; }

        public IServiceProvider Services { get; set; }

        private IReporter _reporter;
        public IReporter Reporter
        {
            get
            {
                if (_reporter == null)
                {
                    _reporter = Services.GetRequiredService<IReporter>();
                }
                return _reporter;
            }
            set { _reporter = value; }
        }

        public DnvmSettings Settings => Services.GetRequiredService<DnvmSettings>();
    }
}