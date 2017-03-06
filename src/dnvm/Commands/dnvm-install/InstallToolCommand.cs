using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class InstallToolCommand : InstallCommand<ToolAsset>
    {
        private readonly string _name;
        private readonly string _version;

        public InstallToolCommand(string name, string version)
        {
            _name = name;
            _version = version;
        }

        protected override ToolAsset CreateAsset(CommandContext context)
            => new ToolAsset(context.Logger, context.Environment, _name, _version);
    }
}
