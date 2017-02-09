#!/usr/bin/env bash

set -e

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
if [[ "$(uname)" == "Darwin" ]]; then
    if brew ls --versions dnvm >/dev/null ; then
        brew outdated libyaml
        [[ $? != 0 ]] && brew install dnvm --without-sdk
    else
        brew tap natemcmaster/dnvm
        brew install dnvm --without-sdk
    fi

    brew outdated libyaml
    [[ $? != 0 ]] && brew install libyaml
fi

dnvm install

mkdir -p artifacts/log/

dotnet restore /nologo dnvm.sln
dotnet publish build.proj /nologo /m /fl /flp:LogFile=artifacts/log/msbuild.log "$@"

echo 'Build succeeded'
