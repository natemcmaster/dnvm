using DotNet.Assets;

namespace DotNet.Commands
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
            => new ToolAsset(context.Reporter, context.Environment, _name, _version);
    }
}