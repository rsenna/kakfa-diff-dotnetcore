Write-Host "Running unit-tests (Debug configuration)"
dotnet vstest ((ls -Recurse *.Test.Unit.dll | % FullName) -Match "\\bin\\Debug\\")
