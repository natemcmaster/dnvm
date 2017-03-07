using System.Collections.Generic;
using System.Runtime.InteropServices;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class RemoveRuntimeCommand : RemoveCommandBase
    {
        private readonly Architecture _arch;
        private readonly string _version;

        public RemoveRuntimeCommand(string version, Architecture arch, bool force) : base(force)
        {
            _version = version;
            _arch = arch;
        }

        protected override IEnumerable<Asset> CreateAssets(CommandContext context)
            => new[] { new RuntimeAsset(context.Logger, context.Environment, _version, _arch) };
    }
}
