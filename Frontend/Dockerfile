FROM ubuntu:bionic AS build
ARG GATSBY_BACKEND_URL
RUN apt-get update && apt-get install -y curl
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash -
RUN apt-get install -y nodejs
RUN npm install -g gatsby
WORKDIR /app
COPY . .
RUN npm ci
RUN gatsby build

FROM nginx
COPY --from=build /app/public /usr/share/nginx/html
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf