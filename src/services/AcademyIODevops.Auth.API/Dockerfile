# Stage 1: runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5077

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os csproj primeiro para aproveitar cache do restore
COPY src/services/AcademyIODevops.Auth.API/AcademyIODevops.Auth.API.csproj src/services/AcademyIODevops.Auth.API/
COPY src/building-blocks/AcademyIODevops.WebAPI.Core/AcademyIODevops.WebAPI.Core.csproj src/building-blocks/AcademyIODevops.WebAPI.Core/
COPY src/building-blocks/AcademyIODevops.Core/AcademyIODevops.Core.csproj src/building-blocks/AcademyIODevops.Core/
COPY src/building-blocks/AcademyIODevops.MessageBus/AcademyIODevops.MessageBus.csproj src/building-blocks/AcademyIODevops.MessageBus/

# Restaura pacotes
RUN dotnet restore "src/services/AcademyIODevops.Auth.API/AcademyIODevops.Auth.API.csproj"

# Copia todo o código fonte
COPY src/ .

# Build e publish no mesmo stage
WORKDIR /src/services/AcademyIODevops.Auth.API
RUN dotnet publish "AcademyIODevops.Auth.API.csproj" -c Release -o /app/publish

# Stage 3: container final
FROM base AS final
ENV ASPNETCORE_URLS="http://+:5077"
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyIODevops.Auth.API.dll"]
