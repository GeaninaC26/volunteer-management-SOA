# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ./api-gateway/ApiGateway.csproj ./api-gateway/
COPY ./shared/VolunteerManagement.Shared.csproj ./shared/
RUN dotnet restore ./api-gateway/ApiGateway.csproj

# Copy the rest of the code and build
COPY ./api-gateway/ ./api-gateway/
COPY ./shared/ ./shared/
RUN dotnet publish ./api-gateway/ApiGateway.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

# Expose port
EXPOSE 8080

ENTRYPOINT ["dotnet", "ApiGateway.dll"]
