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
            var app = new CommandLineApplication
            {
                Name = "dnvm",
                FullName = ".NET Core Version Manager",
                Out = new StringWriter(_helpText)
            };

            app.VersionOption("--version", GetVersion);
            app.HelpOption();

            var optVerbose =
                app.Option("-v|--verbose", "Use verbose output",
                CommandOptionType.NoValue, inherited: true);

            InitCommand(app);
            InfoCommand(app);
            InstallCommand(app);
            ListCommand(app);

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