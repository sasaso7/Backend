# Use the .NET 8 SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY BankBackend.csproj ./
RUN dotnet restore BankBackend.csproj

# Copy the entire project and build it
COPY . ./
RUN dotnet build BankBackend.csproj -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish BankBackend.csproj -c Release -o /app/publish /p:UseAppHost=false

# Use the ASP.NET Core runtime image for the final stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy the published application to the final stage
COPY --from=publish /app/publish ./

# Set the ASPNETCORE_URLS environment variable
ENV ASPNETCORE_URLS=http://+:8080

# Add health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

# Run the application
ENTRYPOINT ["dotnet", "BankBackend.dll"]