using System.Collections.Generic;
using System.Linq;
using NuGet.Versioning;

namespace DotNet.VersionManager.Files
{
    public static class FrameworkExtensions
    {
        public static NetCoreFramework LatestNetCore(this IEnumerable<Framework> frameworks)
        {
            return frameworks
                .OfType<NetCoreFramework>()
                .Select(sdk => new { v = SemanticVersion.Parse(sdk.Version), sdk })
                .OrderByDescending(i => i.v)
                .First()
                .sdk;
        }
    }
}
