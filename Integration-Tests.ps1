Write-Host "Running integration-tests (Debug configuration)"
Write-Host

dotnet build -c Debug Test.Integration\Test.Integration.csproj

$Env:KAFKA_DIFF_ENV = "local"
dotnet vstest Test.Integration\bin\Debug\netcoreapp2.0\Kafka.Diff.Test.Integration.dll
