# Stage 1: Use the ASP.NET runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Stage 2: Use the SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
# Copy project files
COPY ["InvoiceManagement.API/InvoiceManagement.API.csproj", "InvoiceManagement.API/"]
COPY ["InvoiceManagement.Application/InvoiceManagement.Application.csproj", "InvoiceManagement.Application/"]
COPY ["InvoiceManagement.Domain/InvoiceManagement.Domain.csproj", "InvoiceManagement.Domain/"]
COPY ["InvoiceManagement.Infrastructure/InvoiceManagement.Infrastructure.csproj", "InvoiceManagement.Infrastructure/"]
# Restore dependencies
RUN dotnet restore "InvoiceManagement.API/InvoiceManagement.API.csproj"
# Copy all source code
COPY . .
WORKDIR "/src/InvoiceManagement.API"
# Build the project
RUN dotnet build "InvoiceManagement.API.csproj" -c Release -o /app/build

# Stage 3: Publish the project
FROM build AS publish
RUN dotnet publish "InvoiceManagement.API.csproj" -c Release -o /app/publish

# Stage 4: Create the final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvoiceManagement.API.dll"]