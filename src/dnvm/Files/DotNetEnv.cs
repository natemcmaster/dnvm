using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNet.Files
{
    public class DotNetEnv
    {
        private readonly DirectoryInfo _binRoot;
        private readonly DirectoryInfo _sdkRoot;
        private readonly DirectoryInfo _root;
        private readonly DirectoryInfo _fxRoot;
        private readonly DirectoryInfo _toolsRoot;

        public DotNetEnv(string name, DirectoryInfo root)
        {
            Name = Ensure.NotNullOrEmpty(name, nameof(name));
            _root = Ensure.NotNull(root, nameof(root));

            _binRoot = new DirectoryInfo(Path.Combine(_root.FullName, "bin"));
            _sdkRoot = new DirectoryInfo(Path.Combine(_root.FullName, "sdk"));
            _fxRoot = new DirectoryInfo(Path.Combine(_root.FullName, "shared"));
            _toolsRoot = new DirectoryInfo(Path.Combine(_root.FullName, "tools"));
        }

        // can have mixed case, but should always be .ToLower()-ed when checking the disk
        public string Name { get; }
        public string Root => _root.FullName;
        public string BinRoot => _binRoot.FullName;
        public string FxRoot => _fxRoot.FullName;
        public string SdkRoot => _sdkRoot.FullName;
        public string ToolsRoot => _toolsRoot.FullName;

        public IEnumerable<Framework> Frameworks
        {
            get
            {
                _fxRoot.Refresh();
                if (!_fxRoot.Exists)
                {
                    return Enumerable.Empty<Framework>();
                }
                return from fx in _fxRoot.GetDirectories()
                       from version in fx.GetDirectories()
                       select new NetCoreFramework(version);
            }
        }

        public IEnumerable<NetCoreSdk> Sdks
        {
            get
            {
                _sdkRoot.Refresh();
                if (!_sdkRoot.Exists)
                {
                    return Enumerable.Empty<NetCoreSdk>();
                }
                return _sdkRoot.GetDirectories().Select(d => new NetCoreSdk(d));
            }
        }

        public IEnumerable<NetCoreTool> Tools
        {
            get
            {
                _toolsRoot.Refresh();
                if (!_toolsRoot.Exists)
                {
                    return Enumerable.Empty<NetCoreTool>();
                }
                return from tools in _toolsRoot.GetDirectories()
                       from versions in tools.GetDirectories()
                       select new NetCoreTool(versions);
            }
        }
    }
}