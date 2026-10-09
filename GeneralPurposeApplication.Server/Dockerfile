FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["GeneralPurposeApplication.Server/GeneralPurposeApplication.Server.csproj", "GeneralPurposeApplication.Server/"]
COPY ["GeneralPurposeApplication.Application/GeneralPurposeApplication.Application.csproj", "GeneralPurposeApplication.Application/"]
COPY ["GeneralPurposeApplication.Domain/GeneralPurposeApplication.Domain.csproj", "GeneralPurposeApplication.Domain/"]
COPY ["GeneralPurposeApplication.Infrastructure/GeneralPurposeApplication.Infrastructure.csproj", "GeneralPurposeApplication.Infrastructure/"]

RUN dotnet restore "GeneralPurposeApplication.Server/GeneralPurposeApplication.Server.csproj"

COPY . .

WORKDIR "/src/GeneralPurposeApplication.Server"

RUN dotnet publish "GeneralPurposeApplication.Server.csproj" -c Release -o /app/publish -p:DockerBuild=true

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080

HEALTHCHECK --interval=10s --timeout=5s --start-period=30s --retries=5 \
  CMD curl --fail --silent http://localhost:8080/api/ready || exit 1

ENTRYPOINT ["dotnet", "GeneralPurposeApplication.Server.dll"]