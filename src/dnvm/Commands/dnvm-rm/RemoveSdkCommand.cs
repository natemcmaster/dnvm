using System.Runtime.InteropServices;
using DotNet.Assets;

namespace DotNet.Commands
{
    public class RemoveSdkCommand : RemoveCommand<SdkAsset>
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public RemoveSdkCommand(string version, Architecture arch, bool force) : base(force)
        {
            _version = version;
            _arch = arch;
        }

        protected override SdkAsset CreateAsset(CommandContext context)
        {
            return new SdkAsset(context.Reporter, context.Environment, _version, _arch);
        }
    }
}
