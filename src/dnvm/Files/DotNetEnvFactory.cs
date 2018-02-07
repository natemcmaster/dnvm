using System.IO;

namespace DotNet.VersionManager.Files
{
    class DotNetEnvFactory
    {
        private DnvmSettings _settings;
        public DotNetEnvFactory(DnvmSettings settings)
        {
            _settings = settings;
        }

        public DotNetEnv CreateFromConfig(ConfigFile file)
            => Create(file.EnvName ?? FileConstants.GlobalEnvName);

        public DotNetEnv CreateDefault()
            => Create(FileConstants.GlobalEnvName);

        private DotNetEnv Create(string envName)
        {
            return new DotNetEnv(
                envName,
                new DirectoryInfo(Path.Combine(_settings.EnvRoot.FullName, envName)));
        }
    }
}
