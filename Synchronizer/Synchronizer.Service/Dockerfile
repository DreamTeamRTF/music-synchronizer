FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Synchronizer.Service/Synchronizer.Service.csproj", "Synchronizer.Service/"]
RUN dotnet restore "Synchronizer.Service/Synchronizer.Service.csproj"
COPY . .
WORKDIR "/src/Synchronizer.Service"
RUN dotnet build "Synchronizer.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Synchronizer.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Synchronizer.Service.dll"]
