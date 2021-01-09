FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
COPY NuGet.Config ./ 
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .

# The line below is only necessary if we want to configure settings using
# an appsettings.json file (e.g. for convenience in a development environment).
# Otherwise, the project assumes we are using environment variables
# COPY appsettings.json ./

ENTRYPOINT ["dotnet", "TarkovLensBot.dll"]