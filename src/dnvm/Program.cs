using System;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DotNet.VersionManager.Commands;
using DotNet.VersionManager.Files;
using DotNet.VersionManager.IO;
using Microsoft.Extensions.Logging;
using McMaster.Extensions.CommandLineUtils;

namespace DotNet.VersionManager
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                return await new Program(PhysicalConsole.Singleton, Directory.GetCurrentDirectory())
                    .RunAsync(args);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return Error;
            }
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

            var options = CommandLine.Parse(args);
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

            if (context == null) return Error;

            context.Logger.LogDebug($"Using environment '{context.Environment.Name}'");

            try
            {
                await options.Command.ExecuteAsync(context);

                if (context.Result == Result.Incomplete)
                {
                    throw new InvalidOperationException($"Command finished without setting the {nameof(CommandContext)}.{nameof(CommandContext.Result)} value");
                }

                return context.Result == Result.Okay
                    ? OK
                    : Error;
            }
            catch (Exception ex)
            {
                context.Logger.LogCritical(ex.ToString());
                return Error;
            }
        }

        private CommandContext CreateContext(CommandLine options)
        {
            var context = new CommandContext
            {
                CancellationToken = _cts.Token,
                Console = _console,
                WorkingDir = _workingDir,
                Logger = new ConsoleLogger(_console, options.IsVerbose),
                Settings = DnvmSettings.Load(),
            };

            var envFactory = new DotNetEnvFactory(context.Settings);
            var configFileFactory = new ConfigFileFactory();
            var configFile = configFileFactory.FindFile(context.WorkingDir);

            if (configFile != null)
            {
                ConfigFile config;
                try
                {
                    config = configFileFactory.Create(configFile);
                }
                catch (FormatException ex)
                {
                    context.Logger.LogError($"Config file '{configFile}' has an invalid format. {ex.Message}");
                    return null;
                }

                context.Environment = envFactory.CreateFromConfig(config);
                context.ConfigFile = config;
            }
            else
            {
                context.Environment = envFactory.CreateDefault();
            }

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
