FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["SimpleService/SimpleService.csproj", "SimpleService/"]
RUN dotnet restore "SimpleService/SimpleService.csproj"
COPY . .
WORKDIR "/src/SimpleService"
RUN dotnet build "SimpleService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SimpleService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SimpleService.dll", "--urls", "http://*:5000"]
