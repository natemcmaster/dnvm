Config files
============

## `$HOME/.dnvmrc`

This is an .ini file that contains the following settings for how dnvm is configured.

### Settings

feeds


## `.dotnet`

This file can be placed anywhere in the filesystem to control directory-specific settings.
This file is in YAML.

```yml
# cli  = dotnet-cli
# format
cli: cli_version

# fx = shared frameworks
# a list of frameworks to install.
# Item formats:
#   fx_name@version
# If fx_name is absent, it is inferred to be Microsoft.NETCore.App
fx:
 - fx_name@version1
 - fx_name@version2
 - version3

 # examples (both are equivalent)
 - Microsoft.NETCore.App@1.0.1
 - 1.0.1
 - SomeCustom.SharedFramework@99.99.99
```