FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Adapter/Driven/TechLanches.Pagamento.Adapter.AWS/TechLanches.Pagamento.Adapter.AWS.csproj", "TechLanches.Pagamento.Adapter.AWS/"]
COPY ["Adapter/Driven/TechLanches.Pagamento.Adapter.ACL.QrCode/TechLanches.Pagamento.Adapter.ACL.QrCode.csproj", "TechLanches.Pagamento.Adapter.ACL.QrCode/"]
COPY ["Adapter/Driver/TechLanches.Pagamento.Adapter.API/TechLanches.Pagamento.Adapter.API.csproj", "TechLanches.Pagamento.Adapter.API/"]
COPY ["Adapter/Driven/TechLanches.Pagamento.Adapter.DynamoDB/TechLanches.Pagamento.Adapter.DynamoDB.csproj", "TechLanches.Pagamento.Adapter.DynamoDB/"]
COPY ["Core/TechLanches.Pagamento.Domain/TechLanches.Pagamento.Domain.csproj", "TechLanches.Pagamento.Domain/"]
COPY ["Core/TechLanches.Pagamento.Core/TechLanches.Pagamento.Core.csproj", "TechLanches.Pagamento.Core/"]
COPY ["Core/TechLanches.Pagamento.Application/TechLanches.Pagamento.Application.csproj", "TechLanches.Pagamento.Application/"]
RUN dotnet restore "TechLanches.Pagamento.Adapter.API/TechLanches.Pagamento.Adapter.API.csproj"
COPY . .
WORKDIR "/src/Adapter/Driver/TechLanches.Pagamento.Adapter.API"
RUN dotnet build "TechLanches.Pagamento.Adapter.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TechLanches.Pagamento.Adapter.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TechLanches.Pagamento.Adapter.API.dll"]