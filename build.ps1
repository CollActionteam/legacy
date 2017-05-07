Param(
  [string]$dockerRepo
)

dotnet restore CollAction
dotnet build CollAction -c Release
dotnet publish CollAction -c Release -o bin/netcoreapp1.1/publish
docker build -t $dockerRepo -t collaction CollAction