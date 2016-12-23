using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.Reporting;

namespace DotNet.Packaging
{
    class Downloader
    {
        public Downloader(IReporter reporter)
        {
        }

        public Task FetchAsync(CancellationToken token = default(CancellationToken))
        {
            return Task.CompletedTask;
        }
    }
}
