#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source $DIR/common.sh

if [[ "$(uname)" == "Darwin" ]]; then
    brew outdated cmake || brew install cmake
    brew outdated libyaml || brew install libyaml
    brew tap natemcmaster/dnvm
    brew outdated dnvm || brew install dnvm
fi

__exec dnvm install
