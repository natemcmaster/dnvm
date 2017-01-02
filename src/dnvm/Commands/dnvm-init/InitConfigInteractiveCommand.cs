using System.IO;
using DotNet.Files;

namespace DotNet.Commands
{
    public class InitConfigInteractiveCommand : SyncCommand
    {
        protected override void Execute(CommandContext context)
        {
            var dest = Path.Combine(context.WorkingDir, FileConstants.Config);

            if (context.ConfigFile != null)
            {
                if (File.Exists(dest))
                {
                    context.Reporter.Error("Failed to initialize new environment.");
                    context.Reporter.Error($"The file '{dest}' already exists .");
                    context.Result = Result.Error;
                    return;
                }

                context.Reporter.Warn($"A config file already exists at '{context.ConfigFile.FilePath}'.");
                context.Reporter.Warn("Adding the new config file here will override this file's settings.");
            }

            context.Console.Write("Environment name (default): ");

            var name = context.Console.In.ReadLine()?.Trim();
            name = string.IsNullOrEmpty(name)
                ? "default"
                : name;

            var config = new ConfigFile
            {
                Environment = name
            };

            using (var file = new FileStream(dest, FileMode.CreateNew))
            using (var writer = new StreamWriter(file))
            {
                new ConfigFileYamlWriter().Write(writer, config);
            }

            Directory.CreateDirectory(Path.Combine(context.Settings.EnvRoot.FullName, name));

            context.Result = Result.Okay;
        }
    }
}