# 1. Stage: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Копируем csproj и восстанавливаем зависимости (улучшает кэширование)
COPY *.csproj ./
RUN dotnet restore

# Копируем всё и билдим
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# 2. Stage: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime

WORKDIR /app

# Копируем из билд-стадии
COPY --from=build /app/publish ./

# Порт, который слушает .NET по умолчанию (Render перебрасывает через $PORT)
ENV ASPNETCORE_URLS=http://+:$PORT

# Запуск
ENTRYPOINT ["dotnet", "BookStore.dll"]
