using System;

namespace Microsoft.Extensions.CommandLineUtils
{
    static class CommandLineExtension
    {
        public static void HelpOption(this CommandLineApplication app)
            => app.HelpOption("-h|-?|--help");

        public static CommandLineApplication Command(this CommandLineApplication app, string name, string description, Action<CommandLineApplication> config)
            => app.Command(name, c =>
                {
                    c.Out = app.Out;
                    c.Error = app.Error;
                    c.Description = description;
                    c.HelpOption();
                    config(c);
                });

        public static void OnExecute(this CommandLineApplication app, Action config)
        {
            app.OnExecute(() =>
            {
                config();
                return 0;
            });
        }

        public static string IfNotNull(this CommandArgument arg)
        {
            if (arg.Value == null)
            {
                throw new CommandParsingException(null, $"Command argument '{arg.Name}' is missing.");
            }
            return arg.Value;
        }

        public static string IfNotNullOrEmpty(this CommandArgument arg)
        {
            if (string.IsNullOrWhiteSpace(arg.Value))
            {
                throw new CommandParsingException(null, $"Command argument '{arg.Name}' is missing or empty.");
            }
            return arg.Value;
        }
    }
}