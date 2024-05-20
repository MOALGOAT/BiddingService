FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /app

COPY BiddingService/BiddingServiceAPI/*.csproj BiddingServiceAPI/
WORKDIR /app/BiddingServiceAPI
RUN dotnet restore

COPY BiddingService/BiddingServiceAPI/. .
RUN dotnet publish -c Release -o /app/published-app

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/published-app .
ENTRYPOINT ["dotnet", "BiddingServiceAPI.dll"]