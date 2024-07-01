#!/usr/bin/env bash
bash --version 2>&1 | head -n 1

set -eo pipefail

cd "$(dirname "${BASH_SOURCE[0]}")"

export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
export DOTNET_MULTILEVEL_LOOKUP=0

dotnet tool update Ewn.Nuke.Tool.Library --prerelease -g

build-library "$@" --root --solution "$(ls *.sln)"
