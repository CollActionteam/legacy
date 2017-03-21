FROM microsoft/aspnetcore:1.1.1
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/netcoreapp1.1} .
ENTRYPOINT ["dotnet", "CollAction.dll"]