using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class RemoveToolCommand : RemoveCommand<ToolAsset>
    {
        private readonly string _name;
        private readonly string _version;

        public RemoveToolCommand(string name, string version, bool force)
            : base(force)
        {
            _name = name;
            _version = version;
        }

        protected override ToolAsset CreateAsset(CommandContext context)
            => new ToolAsset(context.Logger, context.Environment, _name, _version);
    }
}
