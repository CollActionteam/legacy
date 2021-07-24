FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
ARG BUILD_CONFIG=Release
ARG DOTNET_CLI_TELEMETRY_OPTOUT=1
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
RUN dotnet build CollAction.csproj -c $BUILD_CONFIG -o /app/build
RUN dotnet publish CollAction.csproj -c $BUILD_CONFIG -o /app/publish
RUN if [ "$BUILD_CONFIG" = "Debug" ] ; then dotnet dev-certs https -ep /app/publish/collaction.pfx -p test ; else echo skipping ssl ; fi

FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "CollAction.dll"] 