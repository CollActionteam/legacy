FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get update && apt-get install -y nodejs build-essential
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
RUN dotnet build CollAction.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish CollAction.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CollAction.dll"]