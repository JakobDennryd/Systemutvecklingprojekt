# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source
COPY . .
RUN dotnet restore "./Weather.Report.csproj" --disable-parallel
RUN dotnet publish "./Weather.Report.csproj" -c release -o /app --no-restore

# Serve Stage
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal
WORKDIR /app
COPY --from=build /app ./

EXPOSE 80
EXPOSE 443
 
ENTRYPOINT ["dotnet", "Weather.Report.dll"]