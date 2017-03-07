#!/usr/bin/env sh

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source $DIR/common.sh

if [ "$(uname)" = "Darwin" ]; then
    if ! [ "$TRAVIS" = true ]; then
        __exec brew install cmake
        brew outdated cmake || __exec brew install cmake
    fi

    __exec brew install libyaml
    brew outdated libyaml || __exec brew install libyaml

    __exec brew tap natemcmaster/dnvm
    __exec brew install dnvm --without-sdk
    brew outdated dnvm || __exec brew install dnvm
fi

__exec dnvm install
