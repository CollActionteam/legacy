FROM ubuntu:bionic AS build
RUN apt update && apt install -y curl
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt install -y nodejs
RUN npm install -g gatsby
WORKDIR /app
COPY . .
RUN npm ci
RUN gatsby build

FROM nginx
COPY --from=build /app/public /usr/share/nginx/html