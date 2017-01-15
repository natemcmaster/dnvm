#!/usr/bin/env bash

set -e

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
dotnet_version=$(<./toolsversion.txt)
DOTNET_HOME=/usr/local/share/dnvm/environments/default
if [[ ! -d "$DOTNET_HOME/sdk/$dotnet_version" ]] ; then
    ../scripts/dotnet-install.sh --install-dir $DOTNET_HOME --version $dotnet_version
fi
if [[ ! -d "$DOTNET_HOME/shared/Microsoft.NETCore.App/1.1.0" ]] ; then
    ../scripts/dotnet-install.sh --install-dir $DOTNET_HOME --version 1.1.0 --channel release/1.1.0 --shared-runtime
fi

mkdir -p artifacts/log/

$DOTNET_HOME/dotnet restore /nologo dnvm.sln
$DOTNET_HOME/dotnet publish build.proj /nologo /m /fl /flp:LogFile=artifacts/log/msbuild.log "$@"

echo 'Build succeeded'
