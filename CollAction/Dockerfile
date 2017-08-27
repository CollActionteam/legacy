FROM microsoft/dotnet:1.1.2-sdk
ARG source
COPY . /app
WORKDIR /app
RUN dotnet restore
VOLUME ["/app/wwwroot/usercontent/"]
ENTRYPOINT [ "dotnet" ]
CMD [ "CollAction.dll" ]
