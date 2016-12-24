namespace DotNet.Reporting
{
    public class IndentingReporter : IReporter
    {
        private readonly IReporter _wrapped;
        private readonly object _writelock = new object();
        private readonly string _indent;

        public IndentingReporter(IReporter wrapped, int spaces)
        {
            Ensure.InRange(spaces, nameof(spaces), 1, 500);

            _wrapped = Ensure.NotNull(wrapped, nameof(wrapped));
            _indent = new string(' ', spaces);
        }

        public virtual void Verbose(string message)
            => _wrapped.Verbose(Indent(message));

        public virtual void Output(string message)
            => _wrapped.Output(Indent(message));

        public virtual void Warn(string message)
            => _wrapped.Warn(Indent(message));

        public virtual void Error(string message)
            => _wrapped.Error(Indent(message));

        private string Indent(string message)
        {
            if (message == null)
            {
                return null;
            }

            return _indent + message;
        }
    }
}