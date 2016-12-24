using System.Threading;
using System.Threading.Tasks;

namespace DotNet.Assets
{
    public abstract class Asset
    {
        public abstract Task InstallAsync(CancellationToken cancellationToken);
    }
}