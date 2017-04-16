Param(
  [string]$dockerRepo
)

dotnet restore CollAction
dotnet build CollAction -c Release
dotnet publish CollAction -c Release
docker build -t $dockerRepo -t collaction CollAction