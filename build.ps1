$ErrorActionPreference = 'Stop'

Set-Location -LiteralPath $PSScriptRoot

$env:DOTNET_SKIP_FIRST_TIME_EXPERIENCE = '1'
$env:DOTNET_CLI_TELEMETRY_OPTOUT = '1'
$env:DOTNET_MULTILEVEL_LOOKUP = '0'

dotnet tool update Ewn.Nuke.Tool.Library --prerelease -g
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

build-library @args --root --solution @(Get-ChildItem *.sln).Name
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }
