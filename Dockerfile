FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /service
COPY ["DockerGui.Service/DockerGui.Service.csproj", ""]
RUN dotnet restore "./DockerGui.Service.csproj"
COPY . .
WORKDIR "/service/."
RUN dotnet build "DockerGui.Service/DockerGui.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DockerGui.Service/DockerGui.Service.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerGui.Service.dll"]