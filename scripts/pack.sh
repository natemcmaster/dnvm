#!/usr/bin/env bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source $DIR/common.sh

if [[ "$(uname)" == "Darwin" ]]; then
    os='macos'
else
    os='linux'
fi

mkdir -p $DIR/../artifacts/log/

__exec dotnet msbuild $DIR/../build/$os.pkgproj /t:Build /nologo /m \
    /clp:Summary \
    /fl "/flp:LogFile=$DIR/../artifacts/log/$os.msbuild.log;Verbosity=normal"
