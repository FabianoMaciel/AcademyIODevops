# Stage 1: runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5107
ENV ASPNETCORE_URLS="http://+:5107"

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia só os csproj
COPY src/api-gateways/AcademyIODevops.Bff/AcademyIODevops.Bff.csproj api-gateways/AcademyIODevops.Bff/
COPY src/building-blocks/AcademyIODevops.Core/AcademyIODevops.Core.csproj building-blocks/AcademyIODevops.Core/
COPY src/building-blocks/AcademyIODevops.WebAPI.Core/AcademyIODevops.WebAPI.Core.csproj building-blocks/AcademyIODevops.WebAPI.Core/

# Restore
RUN dotnet restore "api-gateways/AcademyIODevops.Bff/AcademyIODevops.Bff.csproj"

# Copia tudo
COPY src/ .

# Build + publish
RUN dotnet publish "api-gateways/AcademyIODevops.Bff/AcademyIODevops.Bff.csproj" -c Release -o /app/publish

# Stage 3: final image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyIODevops.Bff.dll"]
