Param(
  [string]$dockerRepo
)

$env:NODE_ENV = "production"
dotnet restore CollAction
dotnet build CollAction -c Release
dotnet publish CollAction -c Release -o bin/netcoreapp/publish
docker build -t $dockerRepo -t collaction CollAction