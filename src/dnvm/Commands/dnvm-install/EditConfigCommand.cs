using System.IO;
using DotNet.VersionManager.Files;

namespace DotNet.VersionManager.Commands
{
    public class EditConfigCommand : SyncCommand
    {
        public delegate void EditAction(ConfigFile file);

        private readonly EditAction _action;
        public EditConfigCommand(EditAction action)
        {
            _action = action;
        }

        protected override void Execute(CommandContext context)
        {
            _action(context.ConfigFile);

            using (var file = new FileStream(context.ConfigFile.FilePath, FileMode.OpenOrCreate))
            using (var writer = new StreamWriter(file))
            {
                new ConfigFileYamlWriter().Write(writer, context.ConfigFile);
            }

            context.Result = Result.Okay;
        }
    }
}
