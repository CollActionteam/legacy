FROM ubuntu:focal AS build
ARG REACT_APP_BACKEND_URL
ARG INLINE_RUNTIME_CHUNK=false
ARG SITEMAP_BACKEND_URL
ARG URL

# Initial dependencies
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -y curl gnupg openssl gettext-base

# Generate developer certificate
WORKDIR /app
RUN openssl req -x509 -nodes -days 99999 -newkey rsa:2048 -subj "/C=NL/ST=Noord Holland/L=Amsterdam/O=CollAction/OU=dev/CN=localhost" -keyout /app/collaction.key -out /app/collaction.crt

# Generate nginx conf
COPY nginx.conf.template .
RUN envsubst '$REACT_APP_BACKEND_URL' < ./nginx.conf.template > ./nginx.conf

# Install yarn
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add -
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" > /etc/apt/sources.list.d/yarn.list
RUN apt-get update && apt-get install -y yarn

# Build
COPY . .
RUN yarn install
RUN yarn run build

FROM nginx
COPY --from=build /app/build /usr/share/nginx/html
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/collaction* /etc/nginx/certs/