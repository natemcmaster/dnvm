using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DotNet.VersionManager.Assets
{
    public abstract class Asset
    {
        public abstract string DisplayName { get; }

        public abstract bool IsInstalled { get; }
        public abstract Task<bool> InstallAsync(CancellationToken cancellationToken);
        public abstract bool Uninstall();
        public abstract IEnumerable<Asset> Dependencies { get; }
    }
}
