#!/usr/bin/env bash

set -e

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

mkdir -p artifacts
pushd artifacts
    ../src/muxer/build.sh
    export PATH=$(pwd)/bin:$PATH
    dnvm_home=/usr/local/share/dnvm/environments/default
    if [[ ! -d "$dnvm_home/sdk/1.0.0-preview4-004233" ]] ; then
        ../scripts/dotnet-install.sh --install-dir $dnvm_home --version 1.0.0-preview4-004233
    fi
    if [[ ! -d "$dnvm_home/shared/Microsoft.NETCore.App/1.1.0" ]] ; then
        ../scripts/dotnet-install.sh --install-dir $dnvm_home --version 1.1.0 --channel release/1.1.0 --shared-runtime
    fi
    dotnet restore ../dnvm.sln
    rm bin/dnvm 2>/dev/null || :
    dotnet publish ../src/dnvm/dnvm.csproj -o $(pwd)/pub
    pushd bin
    ln -s ../pub/dnvm ./dnvm
    popd
    echo "App size = $(du -hd 1 pub/ | awk '{print $1}')"
popd

echo 'Build succeeded'