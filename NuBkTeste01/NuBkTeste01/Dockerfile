#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:3.1 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["NuBkTeste01/NuBkTeste01.csproj", "NuBkTeste01/"]
RUN dotnet restore "NuBkTeste01/NuBkTeste01.csproj"
COPY . .
WORKDIR "/src/NuBkTeste01"
RUN dotnet build "NuBkTeste01.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "NuBkTeste01.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "NuBkTeste01.dll"]
