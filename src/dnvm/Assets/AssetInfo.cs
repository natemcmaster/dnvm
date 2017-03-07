using System.Collections.Generic;

namespace DotNet.VersionManager.Assets
{
    public class AssetInfo
    {
        public virtual string Id { get; protected set; }
        public virtual string Version { get; protected set; }

        public virtual string DownloadUrl { get; protected set; }

        public virtual ICollection<AssetInfo> Dependencies { get; protected set; }
            = new List<AssetInfo>();
    }
}
