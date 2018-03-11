Write-Host "Running integration-tests (Debug configuration)"
dotnet vstest Test.Integration\bin\Debug\netcoreapp2.0\Kafka.Diff.Test.Integration.dll
