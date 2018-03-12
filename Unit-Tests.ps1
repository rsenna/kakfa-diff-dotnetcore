Write-Host "Running unit-tests (Debug configuration)"
Write-Host
dotnet build -c Debug
dotnet vstest ((ls -Recurse *.Test.Unit.dll | % FullName) -Match "\\bin\\Debug\\")
