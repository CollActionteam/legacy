FROM microsoft/dotnet:1.1.2-sdk
ARG source
COPY . /app
RUN mkdir -p /root/.config/NuGet
WORKDIR /app
RUN dotnet restore --configfile NuGet.Config
RUN dotnet build
VOLUME ["/app/wwwroot/usercontent/"]
ENTRYPOINT [ "dotnet" ]
CMD [ "CollAction.dll" ]
