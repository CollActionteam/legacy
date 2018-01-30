FROM microsoft/aspnetcore:2.0.5
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/netcoreapp/publish} .
VOLUME ["/app/wwwroot/usercontent/"]
ENTRYPOINT ["dotnet", "CollAction.dll"]