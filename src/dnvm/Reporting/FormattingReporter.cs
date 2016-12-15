// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace DotNet.Reporting
{
    public class FormattingReporter : DefaultReporter
    {
        private readonly IFormatter _verbose;
        private readonly IFormatter _warn;
        private readonly IFormatter _output;
        private readonly IFormatter _error;

        public FormattingReporter(IConsole console,
            IFormatter verbose,
            IFormatter output,
            IFormatter warn,
            IFormatter error)
            : base(console)
        {
            Ensure.NotNull(verbose, nameof(verbose));
            Ensure.NotNull(output, nameof(output));
            Ensure.NotNull(warn, nameof(warn));
            Ensure.NotNull(error, nameof(error));

            _verbose = verbose;
            _output = output;
            _warn = warn;
            _error = error;
        }

        public override void Verbose(string message)
            => Verbose(_verbose.Format(message));

        public override void Output(string message)
            => Output(_output.Format(message));

        public override void Warn(string message)
            => Warn(_warn.Format(message));

        public override void Error(string message)
            => Error(_error.Format(message));
    }
}