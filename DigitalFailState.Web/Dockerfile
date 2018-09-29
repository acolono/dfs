FROM microsoft/dotnet:2.1-sdk-alpine AS build
WORKDIR /app
COPY *.csproj ./
RUN dotnet restore
COPY . .
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime-alpine AS runtime
ENV ASPNETCORE_URLS="http://+:80"
WORKDIR /app
COPY --from=build /app/out ./
HEALTHCHECK CMD wget -O /dev/null -q -T 5 http://localhost/ || exit 1
VOLUME /app/storage
ENTRYPOINT ["dotnet", "DigitalFailState.Web.dll"]
