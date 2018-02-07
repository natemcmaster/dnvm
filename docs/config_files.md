Config files
============

### Settings

## `.dnvm`

This file can be placed anywhere in the filesystem to control directory-specific settings.
This file is in YAML.

```yml
# sdk  = dotnet-sdk
# format
sdk: sdk_version

# runtimes = .NET Core runtimes
runtimes: version

# Can also be a list of multiple versions runtimes to install.
runtimes:
  - version1
  - version2

# tools = .NET Core CLI tools
tools:
  name: version

# envName = environment name
envName: env_name
```

Example:

```yml
envName: myapp
sdk: 1.0.0-preview4-004233
runtimes:
  - 1.0.1
  - 1.1.0
tools:
  dotnet-watch: 1.0.0
  dotnet-user-secrets: stable # can reference a version alias, like 'stable'
```
