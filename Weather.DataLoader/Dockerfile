# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./Weather.DataLoader.csproj" --disable-parallel
RUN dotnet publish "./Weather.DataLoader.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./


ENTRYPOINT ["dotnet", "Weather.DataLoader.dll"] 