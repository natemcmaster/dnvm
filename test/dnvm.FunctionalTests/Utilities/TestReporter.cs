using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions.Internal;
using Xunit.Abstractions;

namespace DotNet.VersionManager.Test
{
    public class TestLogger : ILogger
    {
        private ITestOutputHelper _output;
        public TestLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
            => NullScope.Instance;

        public void Error(string message)
            => _output.WriteLine("err : " + message);

        public bool IsEnabled(LogLevel logLevel)
            => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _output.WriteLine($"{logLevel} : {formatter(state, exception)}");
        }
    }
}
