#!/usr/bin/env bash
dotnet restore CollAction
dotnet publish CollAction -c Release
docker-compose -f docker-compose.yml -f docker-compose.vs.release.yml build
docker save collaction > CollAction.tar