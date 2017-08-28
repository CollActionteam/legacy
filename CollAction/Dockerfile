FROM microsoft/dotnet:1.1.2-sdk
ARG source
COPY . /app
RUN mkdir -p /root/.config/NuGet
COPY ./NuGet.Config /root/.config/NuGet/NuGet.Config
WORKDIR /app
RUN dotnet restore
VOLUME ["/app/wwwroot/usercontent/"]
ENTRYPOINT [ "dotnet" ]
CMD [ "CollAction.dll" ]
