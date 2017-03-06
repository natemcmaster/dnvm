#!/usr/bin/env bash

CYAN="\033[0;36m"
RESET="\033[0m"

set -e

__exec() {
    local cmd=$1
    shift
    echo -e "${CYAN}> $cmd $@${RESET}"
    $cmd $@
    if [[ $? != 0 ]]; then
        exit $?
    fi
}

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

__exec dnvm install

mkdir -p artifacts/log/

config='/p:Configuration=Release'

__exec dotnet restore /nologo dnvm.sln

for proj in test/*/*.csproj; do
    __exec dotnet test $proj $config
done

__exec dotnet publish build.proj /nologo /m \
    /fl /flp:LogFile=artifacts/log/msbuild.log \
    $config

echo 'Build succeeded'
