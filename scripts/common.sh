#!/usr/bin/env bash

RED='\033[0;31m'
GREEN='\033[0;32m'
CYAN='\033[0;36m'
RESET='\033[0m'

export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1

set -euo pipefail

__error() {
    echo -e "${RED}$*${RESET}"
}

__exec() {
    local cmd=$1
    shift
    echo -e "${CYAN}> ${cmd} $*${RESET}"
    $cmd "$@"
}
