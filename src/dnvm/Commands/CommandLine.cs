using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DotNet.Files;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLine
    {
        private static readonly Assembly s_thisAssembly = typeof(Program).GetTypeInfo().Assembly;
        private const string Logo = @"
   ___  _  ___   ____  ___
  / _ \/ |/ / | / /  |/  /
 / // /    /| |/ / /|_/ /
/____/_/|_/ |___/_/  /_/
        ";

        private CommandLine()
        { }

        private Exception _error;

        public bool IsError
        {
            get { return _error != null; }
        }

        private readonly StringBuilder _helpText = new StringBuilder();

        public bool IsVerbose { get; private set; }
        public bool IsHelp { get; private set; }
        public ICommand Command { get; private set; }

        public static CommandLine Parse(string[] args)
        {
            var options = new CommandLine();

            options.Global(args);

            return options;
        }

        private void Global(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "dnvm",
                FullName = ".NET Core Version Manager",
                Out = new StringWriter(_helpText)
            };

            app.VersionOption("--version", GetVersion, GetFullVersion);
            app.HelpOption();

            var optVerbose =
                app.Option("-v|--verbose", "Use verbose output",
                CommandOptionType.NoValue, inherited: true);

            app.Command("init", $"Initializes a new '{FileConstants.Config}' config file in the current directory.", InitCommand);
            app.Command("info", "Display information about the current dotnet environment", InfoCommand);
            app.Command("install", "Install things", InstallCommand);
            app.Command("rm", "Removes installed things", RemoveCommand);
            app.Command("search", "Search for available versions", SearchCommand);

            app.OnExecute(() =>
            {
                _helpText.AppendLine(Logo);
                app.ShowHelp();
            });

            try
            {
                app.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                _error = ex;
                return;
            }

            IsVerbose = optVerbose.HasValue();
            IsHelp = app.IsShowingInformation;
        }

        public string GetErrorText() => _error.Message;
        public string GetHelpText() => _helpText.ToString().Trim(new[] { '\r', '\n' });

        private static string GetVersion()
        {
            var version = s_thisAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? s_thisAssembly.GetName().Version.ToString();

            return version;
        }

        private static string GetFullVersion()
        {
            var commit = s_thisAssembly
                .GetCustomAttributes<AssemblyMetadataAttribute>()
                .FirstOrDefault(a => a.Key == "CommitHash")
                ?.Value;

            return string.IsNullOrEmpty(commit)
                ? GetVersion()
                : $"{GetVersion()}\n{commit}";
        }
    }
}