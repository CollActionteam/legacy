export NODE_ENV=Production
dotnet restore CollAction
dotnet publish CollAction -c Release -o bin/netcoreapp1.1/publish
docker build -t $1 -t collaction CollAction
