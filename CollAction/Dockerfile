FROM microsoft/aspnetcore:1.1.2
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/netcoreapp1.1/publish} .
VOLUME ["/app/wwwroot/usercontent/"]
ENTRYPOINT ["dotnet", "CollAction.dll"]