#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ASBDDS.API/ASBDDS.API.csproj", "ASBDDS.API/"]
COPY ["ExtLibraries/DHCPServer/DHCPServer/Library/DHCPServer.csproj", "ExtLibraries/DHCPServer/DHCPServer/Library/"]
COPY ["ASBDDS.Web.Admin/ASBDDS.Web.Admin.csproj", "ASBDDS.Web.Admin/"]
COPY ["ASBDDS.Web.Shared.Razor/ASBDDS.Web.Shared.Razor.csproj", "ASBDDS.Web.Shared.Razor/"]
COPY ["ASBDDS.Web.Shared/ASBDDS.Web.Shared.csproj", "ASBDDS.Web.Shared/"]
COPY ["ASBDDS.Shared/ASBDDS.Shared.csproj", "ASBDDS.Shared/"]
COPY ["ASBDDS.Web.Client/ASBDDS.Web.Client.csproj", "ASBDDS.Web.Client/"]
COPY ["ExtLibraries/tftp.net/Tftp.Net/Tftp.Net.csproj", "ExtLibraries/tftp.net/Tftp.Net/"]
RUN dotnet restore "ASBDDS.API/ASBDDS.API.csproj"
COPY . .
WORKDIR "/src/ASBDDS.API"

FROM build AS publish
RUN dotnet publish "ASBDDS.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ASBDDS.API.dll"]
