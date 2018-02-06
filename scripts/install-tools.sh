#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source $DIR/common.sh
skip_dnvm=false

while [ $# -gt 0 ]; do
    case $1 in
        --no-dnvm)
            skip_dnvm=true
            ;;
        *)
            __error "Unrecognized option $1"
            exit 1
            ;;
    esac
    shift
done

if [ "$(uname)" = "Darwin" ]; then
    if ! [ "${TRAVIS:-}" = true ]; then
        __exec brew install cmake
        brew outdated cmake || __exec brew install cmake
    fi

    __exec brew install libyaml
    brew outdated libyaml || __exec brew install libyaml

    if [ "$skip_dnvm" != true ]; then
        __exec brew tap natemcmaster/dnvm
        __exec brew install dnvm --without-sdk
        brew outdated dnvm || __exec brew install dnvm
    fi
fi

if [ "$skip_dnvm" != true ]; then
    __exec dnvm install
fi
