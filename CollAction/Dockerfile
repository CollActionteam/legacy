FROM microsoft/dotnet:2.2-sdk AS build
ARG WITH_TEST_SSL=0
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
RUN dotnet build CollAction.csproj --no-restore -c Release -o /app
RUN dotnet publish CollAction.csproj --no-build -c Release -o /app
RUN if [ "$WITH_TEST_SSL" = "1" ] ; then dotnet dev-certs https -ep /app/collaction.pfx -p test ; else echo skipping ssl ; fi

FROM microsoft/dotnet:2.2-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CollAction.dll"] 