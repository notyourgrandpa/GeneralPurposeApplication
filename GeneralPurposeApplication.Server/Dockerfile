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

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "GeneralPurposeApplication.Server.dll"]