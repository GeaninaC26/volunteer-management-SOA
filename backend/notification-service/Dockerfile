# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ./NotificationService.csproj .
RUN dotnet restore ./NotificationService.csproj

# Copy the rest of the code and build
COPY . .
RUN dotnet publish ./NotificationService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "NotificationService.dll"]