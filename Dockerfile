FROM mcr.microsoft.com/dotnet/core/aspnet:3.0-buster-slim AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.0-buster AS build
WORKDIR /service
COPY ["service/DockerGui.csproj", ""]
RUN dotnet restore "./DockerGui.csproj"
COPY . .
WORKDIR "/service/."
RUN dotnet build "service/DockerGui.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "service/DockerGui.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DockerGui.dll"]