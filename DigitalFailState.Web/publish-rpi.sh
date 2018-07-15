#!/bin/sh
rm -rf bin/linux-arm-pub
dotnet publish -r linux-arm -c Release -o bin/linux-arm-pub
