using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class RemoveRuntimeCommand : RemoveCommand<RuntimeAsset>
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public RemoveRuntimeCommand(string version, Architecture arch, bool force) : base(force)
        {
            _version = version;
            _arch = arch;
        }

        protected override RuntimeAsset CreateAsset(CommandContext context)
        {
            return new RuntimeAsset(context.Logger, context.Environment, _version, _arch);
        }
    }
}
