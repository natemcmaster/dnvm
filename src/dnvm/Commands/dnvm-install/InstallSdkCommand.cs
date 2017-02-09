using System.Runtime.InteropServices;
using DotNet.Assets;

namespace DotNet.Commands
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
        {
            return new SdkAsset(context.Reporter, context.Environment, _version, _arch);
        }
    }
}