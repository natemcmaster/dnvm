using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class RemoveFxCommand : RemoveCommand<SharedFxAsset>
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public RemoveFxCommand(string version, Architecture arch, bool force) : base(force)
        {
            _version = version;
            _arch = arch;
        }

        protected override SharedFxAsset CreateAsset(CommandContext context)
        {
            return new SharedFxAsset(context.Logger, context.Environment, _version, _arch);
        }
    }
}
