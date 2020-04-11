FROM ubuntu:bionic AS build
ARG REACT_APP_BACKEND_URL

# Add apt keys
RUN apt-get update && apt-get install -y curl gnupg
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | apt-key add -
RUN curl -sS https://deb.nodesource.com/gpgkey/nodesource.gpg.key | apt-key add -

# Add apt sources
RUN echo "deb https://dl.yarnpkg.com/debian/ stable main" > /etc/apt/sources.list.d/yarn.list
RUN echo 'deb https://deb.nodesource.com/node_12.x bionic main' > /etc/apt/sources.list.d/nodesource.list
RUN echo 'deb-src https://deb.nodesource.com/node_12.x bionic main' >> /etc/apt/sources.list.d/nodesource.list

# Install yarn
RUN apt-get update && apt-get install -y yarn

# Build
WORKDIR /app
COPY . .
RUN yarn install
RUN yarn run build

FROM nginx
COPY --from=build /app/build /usr/share/nginx/html
COPY --from=build /app/nginx.conf /etc/nginx/conf.d/default.conf