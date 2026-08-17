# ============================================================
# Build Stage
# ============================================================

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src

COPY ["TicketBackend.csproj", "./"]

RUN dotnet restore "TicketBackend.csproj"

COPY . .

RUN dotnet publish "TicketBackend.csproj" \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# ============================================================
# Runtime Stage
# ============================================================

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

# ASP.NET Core listens on port 8080
ENV ASPNETCORE_URLS=http://+:8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "TicketBackend.dll"]
