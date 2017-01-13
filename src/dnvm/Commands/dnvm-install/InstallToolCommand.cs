using DotNet.Assets;

namespace DotNet.Commands
{
    public class InstallToolCommand : InstallCommand<ToolAsset>
    {
        private readonly string _name;
        private readonly string _version;

        public InstallToolCommand(string name, string version)
            : base(version)
        {
            _name = name;
            _version = version;
        }

        protected override ToolAsset CreateAsset(CommandContext context)
        {
            return new ToolAsset(context.Reporter, context.Environment, _name, _version);
        }
    }
}