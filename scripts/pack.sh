#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source $DIR/common.sh

if [ "$(uname)" = "Darwin" ]; then
    os='macos'
else
    os='linux'
fi

mkdir -p $DIR/../artifacts/log/

__exec dotnet msbuild $DIR/../build/$os.pkgproj \
    -nologo \
    -m \
    -t:Build \
    -clp:Summary \
    "-bl:$DIR/../artifacts/log/$os.msbuild.binlog"
