# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ./recruitment-service/RecruitmentService.csproj ./recruitment-service/
COPY ./shared/VolunteerManagement.Shared.csproj ./shared/
RUN  dotnet restore ./recruitment-service/RecruitmentService.csproj

# Copy the rest of the code and build
COPY ./recruitment-service/ ./recruitment-service/
COPY ./shared/ ./shared/
RUN dotnet publish ./recruitment-service/RecruitmentService.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RecruitmentService.dll"]