using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNet.Files
{
    public class DotNetEnv
    {
        private readonly DirectoryInfo _sdkRoot;
        private readonly DirectoryInfo _root;
        private readonly DirectoryInfo _sharedRoot;

        public DotNetEnv(string name, DirectoryInfo root)
        {
            Name = Ensure.NotNullOrEmpty(name, nameof(name));
            _root = Ensure.NotNull(root, nameof(root));

            _sdkRoot = new DirectoryInfo(Path.Combine(_root.FullName, "sdk"));
            _sharedRoot = new DirectoryInfo(Path.Combine(_root.FullName, "shared"));
        }

        // can have mixed case, but should always be .ToLower()-ed when checking the disk
        public string Name { get; }
        public string Root => _root.FullName;

        public IEnumerable<Framework> Frameworks
        {
            get
            {
                _sharedRoot.Refresh();
                if (!_sharedRoot.Exists)
                {
                    return Enumerable.Empty<Framework>();
                }
                return _sharedRoot.GetDirectories().SelectMany(d => d.GetDirectories().Select(ds => new NetCoreFramework(ds)));
            }
        }

        public IEnumerable<Cli> Sdks
        {
            get
            {
                _sdkRoot.Refresh();
                if (!_sdkRoot.Exists)
                {
                    return Enumerable.Empty<Cli>();
                }
                return _sdkRoot.GetDirectories().Select(d => new Cli(d));
            }
        }
    }
}