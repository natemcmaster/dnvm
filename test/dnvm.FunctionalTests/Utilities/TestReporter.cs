using DotNet.Reporting;
using Xunit.Abstractions;

namespace DotNet.Test
{
    public class TestReporter : IReporter
    {
        private ITestOutputHelper _output;
        public TestReporter(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Error(string message)
            => _output.WriteLine("err : " + message);
        public void Output(string message)
            => _output.WriteLine("out : " + message);

        public void Verbose(string message)
            => _output.WriteLine("vrbs: " + message);

        public void Warn(string message)
            => _output.WriteLine("warn: " + message);
    }
}