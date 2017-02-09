using System.Runtime.InteropServices;
using DotNet.Assets;

namespace DotNet.Commands
{
    public class InstallFxCommand : InstallCommand<SharedFxAsset>
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public InstallFxCommand(string version, Architecture arch)
        {
            _version = version;
            _arch = arch;
        }

        protected override SharedFxAsset CreateAsset(CommandContext context)
            => new SharedFxAsset(context.Reporter, context.Environment, _version, _arch);
    }
}
