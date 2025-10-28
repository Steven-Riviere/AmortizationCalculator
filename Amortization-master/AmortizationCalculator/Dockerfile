# Étape 1 : build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copier le fichier csproj et restaurer les dépendances
COPY *.csproj ./ 
RUN dotnet restore

# Copier le reste du code et publier
COPY . ./ 
RUN dotnet publish -c Release -o /app/publish

# Étape 2 : runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "AmortizationCalculator.dll"]