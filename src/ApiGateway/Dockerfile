FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY src/ApiGateway/*.csproj src/ApiGateway/
RUN dotnet restore src/ApiGateway/ApiGateway.csproj

COPY src/ApiGateway/ src/ApiGateway/
RUN dotnet publish src/ApiGateway/ApiGateway.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "ApiGateway.dll"]