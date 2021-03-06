using System.IO;
using DotNet.VersionManager.Files;
using McMaster.Extensions.CommandLineUtils;

namespace DotNet.VersionManager.Commands
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
                    context.Logger.Error("Failed to initialize new environment.");
                    context.Logger.Error($"The file '{dest}' already exists .");
                    context.Result = Result.Error;
                    return;
                }

                context.Logger.Warn($"A config file already exists at '{context.ConfigFile.FilePath}'.");
                context.Logger.Warn("Adding the new config file here will override this file's settings.");
            }

            var defaultName = Path.GetFileName(context.WorkingDir);
            context.Console.Write($"Environment name ({defaultName}): ");

            var name = context.Console.In.ReadLine()?.Trim();

            name = string.IsNullOrEmpty(name)
                ? defaultName
                : name;

            var config = new ConfigFile
            {
                EnvName = name
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
