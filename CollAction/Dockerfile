FROM node:10.9-alpine as webpack
WORKDIR /src
COPY CollAction/package.json .
COPY CollAction/package-lock.json .
RUN npm install
COPY CollAction/Frontend ./Frontend
COPY CollAction/webpack.config.js .
COPY CollAction/tsconfig.json .
RUN NODE_ENV=production npm run build

FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
RUN dotnet build CollAction.csproj --no-restore -c Release -o /app
RUN dotnet publish CollAction.csproj --no-build -c Release -o /app
COPY --from=webpack /src/wwwroot/js /app/wwwroot/js

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build /app .
VOLUME ["/app/wwwroot/usercontent/"]
EXPOSE 80
ENTRYPOINT ["dotnet", "CollAction.dll"]
