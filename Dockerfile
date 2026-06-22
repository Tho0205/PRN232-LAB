FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["LAB/PRN232.LAB.API.csproj", "LAB/"]
COPY ["PRN232.LAB.Services/PRN232.LAB.Services.csproj", "PRN232.LAB.Services/"]
COPY ["PRN232.LAB.Repositories/PRN232.LAB.Repositories.csproj", "PRN232.LAB.Repositories/"]
RUN dotnet restore "LAB/PRN232.LAB.API.csproj"

COPY . .
WORKDIR "/src/LAB"
RUN dotnet build "PRN232.LAB.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "PRN232.LAB.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "PRN232.LAB.API.dll"]
