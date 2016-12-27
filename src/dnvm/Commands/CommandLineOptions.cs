using System;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Extensions.CommandLineUtils;

namespace DotNet.Commands
{
    partial class CommandLineOptions
    {
        private const string Logo = @"
   ___  _  ___   ____  ___
  / _ \/ |/ / | / /  |/  /
 / // /    /| |/ / /|_/ /
/____/_/|_/ |___/_/  /_/
        ";

        private CommandLineOptions()
        { }

        private CommandLineApplication _syntax;
        private Exception _error;

        public bool IsError
        {
            get { return _error != null; }
        }

        private readonly StringBuilder _helpText = new StringBuilder();

        public bool IsVerbose { get; private set; }
        public bool IsHelp { get; private set; }
        public ICommand Command { get; private set; }

        public static CommandLineOptions Parse(string[] args)
        {
            var options = new CommandLineOptions();

            options.Global(args);

            return options;
        }

        private void Global(string[] args)
        {
            _syntax = new CommandLineApplication
            {
                Name = "dnvm",
                FullName = ".NET Core Version Manager",
                Out = new StringWriter(_helpText)
            };

            _syntax.VersionOption("--version", GetVersion);
            _syntax.HelpOption();

            var optVerbose =
                _syntax.Option("-v|--verbose", "Use verbose output",
                CommandOptionType.NoValue, inherited: true);

            InfoCommand();
            InstallCommand();
            ListCommand();

            _syntax.OnExecute(() =>
            {
                _helpText.AppendLine(Logo);
                _syntax.ShowHelp();
            });

            try
            {
                _syntax.Execute(args);
            }
            catch (CommandParsingException ex)
            {
                _error = ex;
                return;
            }

            IsVerbose = optVerbose.HasValue();
            IsHelp = _syntax.IsShowingInformation;
        }

        public string GetErrorText() => _error.Message;
        public string GetHelpText() => _helpText.ToString().Trim(new [] { '\r', '\n' });

        private static string GetVersion()
        {
            var assembly = typeof(Program).GetTypeInfo().Assembly;
            var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                ?? assembly.GetName().Version.ToString();

            return version;
        }
    }
}