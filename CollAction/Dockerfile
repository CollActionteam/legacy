# Build webpack bundle 
# Installs all packages and builds a production version of the javascript and css
FROM node:10.9 as webpack
WORKDIR /src
COPY CollAction/package.json .
COPY CollAction/package-lock.json .
RUN npm install
COPY CollAction/Frontend ./Frontend
COPY CollAction/webpack.config.js .
COPY CollAction/tsconfig.json .
RUN NODE_ENV=production npm run build

# Build application
# Installs all packages, compiles the dotnet code and copies the frontend bundle in it
FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /src
COPY CollAction/CollAction.csproj CollAction/
RUN dotnet restore CollAction/CollAction.csproj
COPY . .
WORKDIR /src/CollAction
RUN dotnet build CollAction.csproj --no-restore -c Release -o /app
RUN dotnet publish CollAction.csproj --no-build -c Release -o /app
COPY --from=webpack /src/wwwroot/js /app/wwwroot/js

# Create a runtime
# Copies the published application package to a runtime container
FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine
WORKDIR /app
COPY --from=build /app .
VOLUME ["/app/wwwroot/usercontent/"]
EXPOSE 80
ENTRYPOINT ["dotnet", "CollAction.dll"]
