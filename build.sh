#!/bin/bash

set -e

pushd ~/hot/mono



#dotnet publish --no-restore MSBuild.csproj
#/property:Platform=AMD64

dotnet publish 

#  /property:Platform=AMD64 \
#  /property:RuntimeIdentifer=osx-x64 \
#  /property:PublishSingleFile=true \
#  /property:SelfContained=true \
#  /property:PublishReadyToRun=true

#/property:RuntimeIdentifier=osx
#.12-x64 
#-v d

popd
