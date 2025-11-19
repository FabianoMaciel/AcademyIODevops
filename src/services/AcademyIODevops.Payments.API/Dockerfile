# Stage 1: runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5272  

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os csproj primeiro para aproveitar cache do restore
COPY src/services/AcademyIODevops.Payments.API/AcademyIODevops.Payments.API.csproj ./services/AcademyIODevops.Payments.API/
COPY src/building-blocks/AcademyIODevops.WebAPI.Core/AcademyIODevops.WebAPI.Core.csproj ./building-blocks/AcademyIODevops.WebAPI.Core/
COPY src/building-blocks/AcademyIODevops.Core/AcademyIODevops.Core.csproj ./building-blocks/AcademyIODevops.Core/
COPY src/building-blocks/AcademyIODevops.MessageBus/AcademyIODevops.MessageBus.csproj ./building-blocks/AcademyIODevops.MessageBus/

# Restaura pacotes
RUN dotnet restore "./services/AcademyIODevops.Payments.API/AcademyIODevops.Payments.API.csproj"

# Copia apenas o código fonte do Payments.API e dos building-blocks
COPY src/services/AcademyIODevops.Payments.API/ ./services/AcademyIODevops.Payments.API/
COPY src/building-blocks/AcademyIODevops.WebAPI.Core/ ./building-blocks/AcademyIODevops.WebAPI.Core/
COPY src/building-blocks/AcademyIODevops.Core/ ./building-blocks/AcademyIODevops.Core/
COPY src/building-blocks/AcademyIODevops.MessageBus/ ./building-blocks/AcademyIODevops.MessageBus/

# Build e publish
WORKDIR /src/services/AcademyIODevops.Payments.API
RUN dotnet publish "AcademyIODevops.Payments.API.csproj" -c Release -o /app/publish

# Stage 3: container final
FROM base AS final
ENV ASPNETCORE_URLS="http://+:5272"
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyIODevops.Payments.API.dll"]
