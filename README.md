# dnvm

dotnet environment manager

### Getting started

```sh
brew install dnvm
dnvm install sdk
mkdir MyFirstApp
dotnet new
dotnet restore
dotnet run
```

### Usage

Install .NET Core SDK

```sh
# installs most recent, stable .NET Core SDK
dnvm install sdk
dnvm install sdk stable

# install a specific version
dnvm install sdk 1.0.0-preview4-004233
```

Install a .NET Core runtime

```sh
# installs to installing most recent stable
dnvm install fx
dnvm install fx stable

# install a specific version of .NET Core
dnvm install fx 1.1.0
```

List available versions
```sh
# list versions of .NET Core runtime
dnvm list fx
# list versions of .NET Core SDK
dnvm list sdk
```

Show what is installed
```sh
dnvm info
```

### The dnvm config file (optional)

A file named `.dnvm` can be placed in a project to configure
the versions of .NET Core runtimes, SDKs, and tools that should be installed.

Example:

```yaml
# required: specify an environment name
env: default
# optional: list for
sdk: 1.0.0-preview4-004233
fx:
  - 1.0.1
  - 1.1.0
```

It will define the 'environment' for all `dotnet` and `dnvm` commands
executed in the directory containing the file, or any subdirectories.

Executing `dnvm install` will install all items in the effective `.dnvm` file.