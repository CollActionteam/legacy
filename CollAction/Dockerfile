FROM microsoft/dotnet:2.1-aspnetcore-runtime
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/netcoreapp/publish} .
VOLUME ["/app/wwwroot/usercontent/"]
ENTRYPOINT ["dotnet", "CollAction.dll"]