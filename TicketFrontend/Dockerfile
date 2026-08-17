# ============================================================
# Build Stage
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TicketFrontend.csproj", "./"]

RUN dotnet restore "TicketFrontend.csproj"

COPY . .

RUN dotnet publish "TicketFrontend.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ============================================================
# Runtime Stage
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TicketFrontend.dll"]
