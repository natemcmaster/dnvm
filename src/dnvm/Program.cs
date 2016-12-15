using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using DotNet.Commands;
using DotNet.Files;
using DotNet.Reporting;

namespace DotNet
{
    using Environment = Files.Environment;

    class Program
    {
        static int Main(string[] args)
        {
            return new Program(PhysicalConsole.Instance, Directory.GetCurrentDirectory()).Run(args);
        }

        private const int OK = 0;
        private const int Error = 1;
        private const int ShowInfo = 2;

        private readonly IConsole _console;
        private readonly string _workingDir;

        public Program(IConsole console, string workingDir)
        {
            _console = console;
            _workingDir = workingDir;
        }

        public int Run(string[] args)
        {
            HandleDebugSwitch(ref args);

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

            var context = CreateContext();
            options.Command.Execute(context);

            if (context.Result == Result.Incomplete)
            {
                throw new InvalidOperationException($"Command finished without setting the {nameof(CommandContext)}.{nameof(CommandContext.Result)} value");
            }

            return context.Result == Result.Done
                ? OK
                : Error;
        }

        private CommandContext CreateContext()
        {
            var context = new CommandContext();

            context.Reporter = new DefaultReporter(_console);
            context.Settings = DnvmSettings.Load();
            context.Environment = new Environment(
                FileConstants.GlobalEnvName,
                new DirectoryInfo(Path.Combine(context.Settings.EnvRoot.FullName, FileConstants.GlobalEnvName)));

            return context;
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
