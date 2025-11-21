# Build fázis
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. LÉPÉS: Csak a .csproj fájlokat másoljuk át (a gyorsítótárazás miatt)
# Figyeld meg: megtartjuk a mappaszerkezetet!
COPY ["WebMVC/EduPortal.WebMVC.csproj", "WebMVC/"]
COPY ["EduPortal.Data/EduPortal.Data.csproj", "EduPortal.Data/"]
COPY ["EduPortal.Logic/EduPortal.Logic.csproj", "EduPortal.Logic/"]
COPY ["EduPortal.Repo/EduPortal.Repo.csproj", "EduPortal.Repo/"]

# 2. LÉPÉS: Restore (Csomagok letöltése a fő projektre)
# Mivel a WebMVC hivatkozik a többire, ez lerántja a függőségeket is
RUN dotnet restore "WebMVC/EduPortal.WebMVC.csproj"

# 3. LÉPÉS: A teljes forráskód másolása
COPY . .

# 4. LÉPÉS: Build és Publish
WORKDIR "/src/WebMVC"
RUN dotnet publish "EduPortal.WebMVC.csproj" -c Release -o /app/publish

# Runtime fázis
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "EduPortal.WebMVC.dll"]