using System.Collections.Generic;
using DotNet.VersionManager.Assets;

namespace DotNet.VersionManager.Commands
{
    public class RemoveToolCommand : RemoveCommandBase
    {
        private readonly string _name;
        private readonly string _version;

        public RemoveToolCommand(string name, string version, bool force)
            : base(force)
        {
            _name = name;
            _version = version;
        }

        protected override IEnumerable<Asset> CreateAssets(CommandContext context)
            => new[] { new ToolAsset(context.Logger, context.Environment, _name, _version) };
    }
}
