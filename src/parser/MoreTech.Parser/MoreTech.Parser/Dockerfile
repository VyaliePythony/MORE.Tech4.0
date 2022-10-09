#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["MoreTech.Parser/MoreTech.Parser.csproj", "MoreTech.Parser/"]
RUN dotnet restore "MoreTech.Parser/MoreTech.Parser.csproj"
COPY . .
WORKDIR "/src/MoreTech.Parser"
RUN dotnet build "MoreTech.Parser.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MoreTech.Parser.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MoreTech.Parser.dll"]