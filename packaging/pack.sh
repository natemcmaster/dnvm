#!/usr/bin/env bash

GREEN="\033[0;32m"
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

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

if [[ "$(uname)" == "Darwin" ]]; then
    os='macos'
    if brew ls --versions dnvm >/dev/null ; then
        brew outdated libyaml
        [[ $? != 0 ]] && brew install dnvm --without-sdk
    else
        brew tap natemcmaster/dnvm
        brew install dnvm --without-sdk
    fi

    brew outdated libyaml
    [[ $? != 0 ]] && brew install libyaml
else
    os='linux'
fi

__exec dnvm install

mkdir -p $DIR/../artifacts/log/

__exec dotnet msbuild $DIR/$os.pkgproj /t:Build /nologo /m \
    /clp:Summary \
    /fl "/flp:LogFile=$DIR/../artifacts/log/$os.msbuild.log;Verbosity=normal"
