using System;
using DotNet.Commands;

namespace DotNet.Reporting
{
    class IndentReportingScope : IDisposable
    {
        private readonly IReporter _original;
        private readonly CommandContext _context;

        public IndentReportingScope(CommandContext context, int spaces)
        {
            _context = context;
            _original = _context.Reporter;
            _context.Reporter = new IndentingReporter(_original, spaces);
        }

        public void Dispose()
        {
            _context.Reporter = _original;
        }
    }
}