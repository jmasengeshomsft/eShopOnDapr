# #See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# EXPOSE 8080
# EXPOSE 443

FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the remaining source code
COPY ["src/Services/Catalog/Catalog.AIGEN/product-generator.csproj", "src/Services/Catalog/Catalog.AIGEN/"]
COPY . ./

WORKDIR "/src/src/Services/Catalog/Catalog.AIGEN"

RUN dotnet restore

FROM build AS publish
RUN dotnet publish "product-generator.csproj" -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "product-generator.dll"]