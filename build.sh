#!/bin/bash
#clean out the last publish
rm -rf bin/ obj/

#build
dotnet restore ./gdo.csproj --runtime ubuntu.16.04-x64
dotnet publish --configuration Release ./gdo.csproj --runtime ubuntu.16.04-x64