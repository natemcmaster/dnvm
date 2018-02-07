#!/usr/bin/env bash


DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
source "$DIR/scripts/common.sh"

# True when we need to download dotnet as a script. Set when bootstrapping DNVM
manual_bootstrap=false

while [ $# -gt 0 ]; do
    case $1 in
        --bootstrap:manual|-b:m)
            manual_bootstrap=true
            ;;
        *)
            __error "Unknown arg $1"
            ;;
    esac
    shift
done

if [ "$manual_bootstrap" = true ]; then
    if [ ! -x "$DIR/.dotnet/dotnet" ]; then
        mkdir -p $DIR/.dotnet/
        curl -sSL https://dot.net/v1/dotnet-install.sh -o $DIR/.dotnet/dotnet-install.sh
        bash $DIR/.dotnet/dotnet-install.sh --install-dir $DIR/.dotnet/ --version 2.1.4
    fi

    export PATH="$DIR/.dotnet:$PATH"
fi

$DIR/scripts/install-tools.sh --no-dnvm
$DIR/scripts/pack.sh
$DIR/scripts/test.sh
