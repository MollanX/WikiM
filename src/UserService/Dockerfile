# Этап сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Копируем csproj и восстанавливаем зависимости
COPY src/UserService/*.csproj src/UserService/
RUN dotnet restore src/UserService/UserService.csproj

# Копируем всё остальное и собираем
COPY src/UserService/ src/UserService/
RUN dotnet publish src/UserService/UserService.csproj -c Release -o /app

# Этап запуска
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "UserService.dll"]