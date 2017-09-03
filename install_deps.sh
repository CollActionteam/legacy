docker run -v "${PWD}/CollAction:/app" -w /app microsoft/dotnet:1.1.2-sdk dotnet restore
rm -Rf CollAction/node_moduels
docker run -v "${PWD}/CollAction:/app" -w /app node npm i

