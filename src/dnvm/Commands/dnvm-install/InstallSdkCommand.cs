using System.Collections.Generic;
using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class InstallSdkCommand : InstallCommandBase
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public InstallSdkCommand(string version, Architecture arch)
        {
            _version = version;
            _arch = arch;
        }

        protected override IEnumerable<Asset> CreateAssets(CommandContext context)
            => new []{ new SdkAsset(context.Logger, context.Environment, _version, _arch) };
    }
}
