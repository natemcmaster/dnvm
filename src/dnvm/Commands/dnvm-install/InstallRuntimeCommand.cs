using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class InstallRuntimeCommand : InstallCommand<RuntimeAsset>
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public InstallRuntimeCommand(string version, Architecture arch)
        {
            _version = version;
            _arch = arch;
        }

        protected override RuntimeAsset CreateAsset(CommandContext context)
            => new RuntimeAsset(context.Logger, context.Environment, _version, _arch);
    }
}
