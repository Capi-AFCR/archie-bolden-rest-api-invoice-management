FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["RestAPIInvoiceManagement.API/RestAPIInvoiceManagement.API.csproj", "RestAPIInvoiceManagement.API/"]
COPY ["RestAPIInvoiceManagement.Application/RestAPIInvoiceManagement.Application.csproj", "RestAPIInvoiceManagement.Application/"]
COPY ["RestAPIInvoiceManagement.Infrastructure/RestAPIInvoiceManagement.Infrastructure.csproj", "RestAPIInvoiceManagement.Infrastructure/"]
COPY ["RestAPIInvoiceManagement.Domain/RestAPIInvoiceManagement.Domain.csproj", "RestAPIInvoiceManagement.Domain/"]
RUN dotnet restore "RestAPIInvoiceManagement.API/RestAPIInvoiceManagement.API.csproj"
COPY . .
WORKDIR "/src/RestAPIInvoiceManagement.API"
RUN dotnet build "RestAPIInvoiceManagement.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "RestAPIInvoiceManagement.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ["RestAPIInvoiceManagement.API/InvoiceDb.db", "."]
ENTRYPOINT ["dotnet", "RestAPIInvoiceManagement.API.dll"]