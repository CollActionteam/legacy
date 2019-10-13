FROM microsoft/dotnet:3.0-sdk AS build
ARG BUILD_CONFIG=Release
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
RUN dotnet build CollAction.csproj --no-restore -c $BUILD_CONFIG -o /app
RUN dotnet publish CollAction.csproj --no-build -c $BUILD_CONFIG -o /app
RUN if [ "$BUILD_CONFIG" = "Debug" ] ; then dotnet dev-certs https -ep /app/collaction.pfx -p test ; else echo skipping ssl ; fi

FROM microsoft/dotnet:3.0-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CollAction.dll"] 