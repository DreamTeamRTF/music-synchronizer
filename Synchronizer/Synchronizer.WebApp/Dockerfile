FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Synchronizer.WebApp/Synchronizer.WebApp.csproj", "Synchronizer.WebApp/"]
COPY ["Synchronizer.Core/Synchronizer.Core.csproj", "Synchronizer.Core/"]
COPY ["Synchronizer.DAL/Synchronizer.DAL.csproj", "Synchronizer.DAL/"]
RUN dotnet restore "Synchronizer.WebApp/Synchronizer.WebApp.csproj"
COPY . .
WORKDIR "/src/Synchronizer.WebApp"
RUN dotnet build "Synchronizer.WebApp.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Synchronizer.WebApp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish/. /app/
WORKDIR /app
ENTRYPOINT ["dotnet", "Synchronizer.WebApp.dll"]
