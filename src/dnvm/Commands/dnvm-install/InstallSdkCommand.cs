using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class InstallSdkCommand : InstallCommand<SdkAsset>
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public InstallSdkCommand(string version, Architecture arch)
        {
            _version = version;
            _arch = arch;
        }

        protected override SdkAsset CreateAsset(CommandContext context)
            => new SdkAsset(context.Logger, context.Environment, _version, _arch);
    }
}
