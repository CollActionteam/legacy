# Build application
# Installs all packages, compiles the dotnet code and typescript code
FROM microsoft/dotnet:2.1-sdk AS build
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get update && apt-get install -y nodejs build-essential
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
# Needed because the project npm install in production mode won't install the devDependencies (webpack etc.)
RUN npm install
RUN NODE_ENV=production dotnet build CollAction.csproj --no-restore -c Release -o /app
RUN dotnet publish CollAction.csproj --no-build -c Release -o /app

# Create a runtime
# Copies the published application package to a runtime container
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build /app .
VOLUME ["/app/wwwroot/usercontent/"]
EXPOSE 80
ENTRYPOINT ["dotnet", "CollAction.dll"]
