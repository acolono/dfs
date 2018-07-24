#!/bin/sh
rm -rf bin/linux-arm-pub
dotnet publish -r linux-arm -o bin/linux-arm-pub
