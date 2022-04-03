FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["./WebSite.csproj", ""]
RUN dotnet restore "./WebSite.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "WebSite.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebSite.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
CMD ASPNETCORE_URLS=http://*:$PORT dotnet WebSite.dll
