using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DotNet.Commands;
using DotNet.Files;
using DotNet.Reporting;

namespace DotNet
{
    using System.Threading;
    using Environment = Files.Environment;

    class Program
    {
        static int Main(string[] args)
        {
            return new Program(PhysicalConsole.Instance, Directory.GetCurrentDirectory())
                .RunAsync(args)
                .GetAwaiter()
                .GetResult();
        }

        private const int OK = 0;
        private const int Error = 1;
        private const int ShowInfo = 2;

        private readonly IConsole _console;
        private readonly string _workingDir;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public Program(IConsole console, string workingDir)
        {
            _console = console;
            _workingDir = workingDir;
        }

        public async Task<int> RunAsync(string[] args)
        {
            HandleDebugSwitch(ref args);

            // TODO cancel _cts when CTRL+C

            var options = CommandLineOptions.Parse(args);
            if (options.IsError)
            {
                _console.WriteLine(options.GetErrorText());
                return Error;
            }
            if (options.IsHelp || options.Command == null)
            {
                _console.WriteLine(options.GetHelpText());
                return ShowInfo;
            }

            var context = CreateContext(options);

            context.Reporter.Verbose($"Using environment '{context.Environment.Name}'");

            await options.Command.ExecuteAsync(context);

            if (context.Result == Result.Incomplete)
            {
                throw new InvalidOperationException($"Command finished without setting the {nameof(CommandContext)}.{nameof(CommandContext.Result)} value");
            }

            return context.Result == Result.Done
                ? OK
                : Error;
        }

        private CommandContext CreateContext(CommandLineOptions options)
        {
            var context = new CommandContext();
            context.CancellationToken = _cts.Token;
            context.Reporter = CreateReporter(options.IsVerbose);
            context.Settings = DnvmSettings.Load();
            context.Environment = new Environment(
                FileConstants.GlobalEnvName,
                new DirectoryInfo(Path.Combine(context.Settings.EnvRoot.FullName, FileConstants.GlobalEnvName)));

            return context;
        }

        private IReporter CreateReporter(bool verbose)
        {
            return new ReporterBuilder()
                .WithConsole(_console)
                .Verbose(c => c.WithColor(ConsoleColor.DarkGray).When(() => verbose))
                .Error(c => c.WithColor(ConsoleColor.Red))
                .Build();
        }

        [Conditional("DEBUG")]
        static void HandleDebugSwitch(ref string[] args)
        {
            if (args.Length > 0 && string.Equals("--debug", args[0], StringComparison.OrdinalIgnoreCase))
            {
                args = args.Skip(1).ToArray();
                Console.WriteLine("Waiting for debugger to attach. Press ENTER to continue");
                Console.WriteLine($"Process ID: {Process.GetCurrentProcess().Id}");
                Console.ReadLine();
            }
        }
    }
}
