# Sử dụng base image .NET để chạy
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

# Sử dụng image SDK để build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy các file .csproj để restore packages
COPY ["LAB1/PRN232.LAB1.API.csproj", "LAB1/"]
COPY ["PRN232.LAB1.Services/PRN232.LAB1.Services.csproj", "PRN232.LAB1.Services/"]
COPY ["PRN232.LAB1.Repositories/PRN232.LAB1.Repositories.csproj", "PRN232.LAB1.Repositories/"]
RUN dotnet restore "LAB1/PRN232.LAB1.API.csproj"

# Copy toàn bộ code vào và build
COPY . .
WORKDIR "/src/LAB1"
RUN dotnet build "PRN232.LAB1.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PRN232.LAB1.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LAB1.API.dll"]
