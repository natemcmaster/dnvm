using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DotNet.VersionManager.Files
{
    public class DotNetEnv
    {
        private readonly DirectoryInfo _binRoot;
        private readonly DirectoryInfo _sdkRoot;
        private readonly DirectoryInfo _root;
        private readonly DirectoryInfo _runtimeRoot;
        private readonly DirectoryInfo _toolsRoot;

        public DotNetEnv(string name, DirectoryInfo root)
        {
            Name = Ensure.NotNullOrEmpty(name, nameof(name));
            _root = root ?? throw new ArgumentNullException(nameof(root));

            _binRoot = new DirectoryInfo(Path.Combine(_root.FullName, "bin"));
            _sdkRoot = new DirectoryInfo(Path.Combine(_root.FullName, "sdk"));
            _runtimeRoot = new DirectoryInfo(Path.Combine(_root.FullName, "shared"));
            _toolsRoot = new DirectoryInfo(Path.Combine(_root.FullName, "tools"));
        }

        // can have mixed case, but should always be .ToLower()-ed when checking the disk
        public string Name { get; }
        public string Root => _root.FullName;
        public string BinRoot => _binRoot.FullName;
        public string FxRoot => _runtimeRoot.FullName;
        public string SdkRoot => _sdkRoot.FullName;
        public string ToolsRoot => _toolsRoot.FullName;

        public IEnumerable<NetCoreRuntime> Runtimes
        {
            get
            {
                _runtimeRoot.Refresh();
                if (!_runtimeRoot.Exists)
                {
                    return Enumerable.Empty<NetCoreRuntime>();
                }

                return from runtime in _runtimeRoot.GetDirectories()
                       from version in runtime.GetDirectories()
                       select new NetCoreRuntime(version);
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
