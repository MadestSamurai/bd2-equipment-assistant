param([switch]$Locked,[string]$ClientManaged='')
$ErrorActionPreference='Stop'
& (Join-Path $PSScriptRoot 'build.ps1') -Locked:$Locked
$version=([xml](Get-Content (Join-Path $PSScriptRoot 'Directory.Build.props') -Raw)).Project.PropertyGroup.Version
$dest=Join-Path $PSScriptRoot "dist/v$version";if(Test-Path -LiteralPath $dest){throw 'Release directory already exists'}
$work=Join-Path $PSScriptRoot ('.build/package-'+[Guid]::NewGuid().ToString('N'));$assets=Join-Path $work 'assets';New-Item -ItemType Directory -Path $assets -Force|Out-Null
$previousData=$env:BD2_EQUIPMENT_DATA_ROOT;$previousPython=$env:PYTHONHOME;$previousPythonPath=$env:PYTHONPATH;$previousPath=$env:PATH
$reports=@()
try{
 foreach($flavor in @('Portable','Lite')){
  $self=if($flavor -eq 'Portable'){'true'}else{'false'};$publish=Join-Path $work $flavor;$argsRestore=@();if($Locked){$argsRestore+='-p:RestoreLockedMode=true'}
  & dotnet publish (Join-Path $PSScriptRoot 'desktop/BD2EquipmentAssistant.csproj') -c Release --self-contained $self "-p:SelfContained=$self" "-p:PublishSelfContained=$self" "-p:EnableCompressionInSingleFile=$self" -p:DebugType=None -p:DebugSymbols=false --artifacts-path (Join-Path $work "$flavor-build") --output $publish @argsRestore --nologo
  if($LASTEXITCODE -ne 0){throw 'Publish failed'}
  $files=@(Get-ChildItem -LiteralPath $publish -File);if($files.Count -ne 1 -or $files[0].Name -ne 'BD2EquipmentAssistant.exe' -or $files[0].Length -ge 80MB){throw 'Expected one EXE smaller than 80 MiB'}
  $alone=Join-Path $work "$flavor-alone";New-Item -ItemType Directory -Path $alone|Out-Null;$exe=Join-Path $alone 'EquipmentStandalone.exe';Copy-Item -LiteralPath $files[0].FullName -Destination $exe
  function Run([string[]]$Arguments){$p=Start-Process -FilePath $exe -ArgumentList $Arguments -WorkingDirectory (Get-Location).Path -WindowStyle Hidden -PassThru;if(!$p.WaitForExit(45000)){throw 'Packaged check timed out'};if($p.ExitCode -ne 0){throw ('Packaged check failed: '+($Arguments -join ' '))}}
  $env:BD2_EQUIPMENT_DATA_ROOT=Join-Path $work "$flavor-fresh-data";$env:PYTHONHOME='Z:\not-installed';$env:PYTHONPATH='Z:\not-installed';$env:PATH="$env:WINDIR\System32;$env:WINDIR"
  foreach($mode in @('cold','warm','english')){$folder=Join-Path $work "$flavor-$mode";$switch=if($mode -eq 'english'){'--smoke-en'}else{'--smoke'};Run @($switch,('"'+$folder+'"'));$check=Get-Content (Join-Path $folder 'smoke.json') -Raw -Encoding UTF8|ConvertFrom-Json;if($check.status -ne 'passed' -or $check.engine -ne 'C# in-process'){throw 'Standalone check failed'}}
  $env:PYTHONHOME=$previousPython;$env:PYTHONPATH=$previousPythonPath;$env:PATH=$previousPath
  if(@(Get-ChildItem -LiteralPath $alone -Force).Count -ne 1){throw 'EXE wrote beside itself'}
  if($ClientManaged){$check=Join-Path $work "$flavor-client";Run @('--connection','prepare',('"'+$ClientManaged+'"'),('"'+$check+'"'));if(!(Test-Path -LiteralPath (Join-Path $check 'bridge.json'))){throw 'Client compile missing'};Run @('--connection','taps',('"'+$ClientManaged+'"'),('"'+(Join-Path $PSScriptRoot 'evidence-spec.json')+'"'),('"'+(Join-Path $check 'evidence-config.json')+'"'))}
  $name="BD2EquipmentAssistant-$version-$flavor-win-x64";$bundle=Join-Path $work $name;New-Item -ItemType Directory -Path $bundle|Out-Null
  Copy-Item -LiteralPath $exe -Destination (Join-Path $assets "$name.exe");Copy-Item -LiteralPath $exe -Destination (Join-Path $bundle "$name.exe")
  foreach($file in @('README.md','README.en.md','LICENSE','THIRD_PARTY_NOTICES.md')){Copy-Item -LiteralPath (Join-Path $PSScriptRoot $file) -Destination $bundle}
  foreach($folder in @('docs','licenses')){Copy-Item -LiteralPath (Join-Path $PSScriptRoot $folder) -Destination $bundle -Recurse}
  Compress-Archive -LiteralPath $bundle -DestinationPath (Join-Path $assets "$name.zip") -CompressionLevel Optimal
  $reports+=@{name=$flavor;bytes=$files[0].Length;singleExe=$true;pythonBundled=$false;checks='passed';languages=@('zh-CN','en-US')}
 }
 Get-ChildItem -LiteralPath $assets -File|ForEach-Object{"$((Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash.ToLowerInvariant())  $($_.Name)"}|Set-Content -LiteralPath (Join-Path $assets 'SHA256SUMS.txt') -Encoding ascii
 @{version=$version;flavors=$reports;realGameTouched=$false;clientCompile=[bool]$ClientManaged;runtimeConsumption='source_implemented_pending_runtime'}|ConvertTo-Json -Depth 5|Set-Content -LiteralPath (Join-Path $assets 'release.json') -Encoding UTF8
 $repo=[IO.Path]::GetFullPath($PSScriptRoot)+'\';if(!([IO.Path]::GetFullPath($dest)).StartsWith($repo) -or !([IO.Path]::GetFullPath($assets)).StartsWith($repo)){throw 'Artifact paths must stay in repository'}
 New-Item -ItemType Directory -Path (Split-Path $dest) -Force|Out-Null;Move-Item -LiteralPath $assets -Destination $dest
 Write-Output "Release assets ready: $dest"
}finally{$env:BD2_EQUIPMENT_DATA_ROOT=$previousData;$env:PYTHONHOME=$previousPython;$env:PYTHONPATH=$previousPythonPath;$env:PATH=$previousPath}
