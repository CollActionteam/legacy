NODE_ENV = "production"
dotnet restore CollAction
dotnet build CollAction -c Release
dotnet publish CollAction -c Release -o bin/netcoreapp/publish
docker build -t $1 -t collaction CollAction