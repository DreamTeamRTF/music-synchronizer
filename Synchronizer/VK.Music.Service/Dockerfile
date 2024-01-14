FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 8020
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["VK.Music.Service/VK.Music.Service.csproj", "VK.Music.Service/"]
RUN dotnet restore "VK.Music.Service/VK.Music.Service.csproj"
COPY . .
WORKDIR "/src/VK.Music.Service"
RUN dotnet build "VK.Music.Service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "VK.Music.Service.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "VK.Music.Service.dll"]
