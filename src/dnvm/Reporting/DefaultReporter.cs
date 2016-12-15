using System.IO;

namespace DotNet.Reporting
{
    public class DefaultReporter : IReporter
    {
        private readonly IConsole _console;
        private readonly object _writelock = new object();

        public DefaultReporter(IConsole console)
        {
            _console = Ensure.NotNull(console, nameof(console));
        }

        public virtual void Verbose(string message)
            => Write(_console.Out, message);

        public virtual void Output(string message)
            => Write(_console.Out, message);

        public virtual void Warn(string message)
            => Write(_console.Out, message);

        public virtual void Error(string message)
            => Write(_console.Error, message);

        private void Write(TextWriter writer, string message)
        {
            if (message == null)
            {
                return;
            }

            lock (_writelock)
            {
                writer.WriteLine(message);
            }
        }
    }
}