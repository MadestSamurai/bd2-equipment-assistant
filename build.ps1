param([switch]$Locked)
$ErrorActionPreference='Stop'
$argsRestore=@();if($Locked){$argsRestore+='--locked-mode'}
foreach($project in @('desktop/BD2EquipmentAssistant.csproj','native-tests/BD2Equipment.Tests.csproj')){
 & dotnet restore (Join-Path $PSScriptRoot $project) @argsRestore --nologo
 if($LASTEXITCODE -ne 0){throw 'Restore failed'}
}
& dotnet build (Join-Path $PSScriptRoot 'desktop/BD2EquipmentAssistant.csproj') -c Release --no-restore --nologo
if($LASTEXITCODE -ne 0){throw 'Desktop build failed'}
& dotnet run --project (Join-Path $PSScriptRoot 'native-tests/BD2Equipment.Tests.csproj') -c Release --no-restore
if($LASTEXITCODE -ne 0){throw 'Regression suite failed'}
