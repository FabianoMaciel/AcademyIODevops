# Stage 1: runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Porta do BFF
EXPOSE 5107

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os csproj para cache de restore
COPY src/api-gateways/AcademyIODevops.Bff/AcademyIODevops.Bff.csproj src/api-gateways/AcademyIODevops.Bff/
COPY src/building-blocks/AcademyIODevops.WebAPI.Core/AcademyIODevops.WebAPI.Core.csproj src/building-blocks/AcademyIODevops.WebAPI.Core/
COPY src/building-blocks/AcademyIODevops.Core/AcademyIODevops.Core.csproj src/building-blocks/AcademyIODevops.Core/

# Restore
RUN dotnet restore "src/api-gateways/AcademyIODevops.Bff/AcademyIODevops.Bff.csproj"

# Copia todo o código fonte
COPY src/ .

# Publish
WORKDIR /src/api-gateways/AcademyIODevops.Bff
RUN dotnet publish "AcademyIODevops.Bff.csproj" -c Release -o /app/publish

# Stage 3: container final
FROM base AS final
WORKDIR /app
ENV ASPNETCORE_URLS="http://+:5107"
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyIODevops.Bff.dll"]
