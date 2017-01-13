Config files
============

### Settings

## `.dnvm`

This file can be placed anywhere in the filesystem to control directory-specific settings.
This file is in YAML.

```yml
# env = environment name
# REQUIRED
env: env_name

# sdk  = dotnet-sdk
# format
sdk: sdk_version

# fx = shared frameworks
# the shared runtime frameworks
fx: version

# Can also be a list of multiple versions frameworks to install.
fx:
 - version1
 - version2
```

Example:

```yml
env: myapp
sdk: 1.0.0-preview4-004233
fx:
  - 1.0.1
  - 1.1.0
```