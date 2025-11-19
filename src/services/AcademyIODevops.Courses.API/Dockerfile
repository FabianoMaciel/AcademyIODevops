# Stage 1: runtime base
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
# Porta do microserviço Courses
EXPOSE 5078  

# Stage 2: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia apenas os csproj primeiro para aproveitar cache do restore
COPY src/services/AcademyIODevops.Courses.API/AcademyIODevops.Courses.API.csproj src/services/AcademyIODevops.Courses.API/
COPY src/building-blocks/AcademyIODevops.WebAPI.Core/AcademyIODevops.WebAPI.Core.csproj src/building-blocks/AcademyIODevops.WebAPI.Core/
COPY src/building-blocks/AcademyIODevops.Core/AcademyIODevops.Core.csproj src/building-blocks/AcademyIODevops.Core/

# Restaura pacotes
RUN dotnet restore "src/services/AcademyIODevops.Courses.API/AcademyIODevops.Courses.API.csproj"

# Copia todo o código fonte
COPY src/ .

# Build e publish no mesmo stage
WORKDIR /src/services/AcademyIODevops.Courses.API
RUN dotnet publish "AcademyIODevops.Courses.API.csproj" -c Release -o /app/publish

# Stage 3: container final
FROM base AS final
ENV ASPNETCORE_URLS="http://+:5078"
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AcademyIODevops.Courses.API.dll"]
