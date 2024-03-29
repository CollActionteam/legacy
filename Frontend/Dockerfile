FROM node:buster AS build
ARG REACT_APP_BACKEND_URL
ARG SITEMAP_LOCATION
ARG BUILD_CONFIG
ARG INLINE_RUNTIME_CHUNK=false

# Initial dependencies
RUN DEBIAN_FRONTEND=noninteractive apt-get update && apt-get install -y curl gnupg openssl gettext-base

# Generate developer certificate
WORKDIR /app
RUN openssl req -x509 -nodes -days 99999 -newkey rsa:2048 -subj "/C=NL/ST=Noord Holland/L=Amsterdam/O=CollAction/OU=dev/CN=localhost" -keyout /app/collaction.key -out /app/collaction.crt

# Generate nginx conf
COPY nginx.conf.template .
RUN envsubst '$REACT_APP_BACKEND_URL,$SITEMAP_LOCATION' < ./nginx.conf.template > ./nginx.conf

# Build
COPY . .
RUN yarn
RUN yarn run build

FROM nginx
COPY --from=build /app/build /usr/share/nginx/html
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /app/collaction* /etc/nginx/certs/
