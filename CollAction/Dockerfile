FROM microsoft/aspnetcore:1.0
ARG source
WORKDIR /app
EXPOSE 80
COPY ${source:-bin/netcoreapp1.0/publish} .
ENTRYPOINT ["dotnet", "Backend.dll"]