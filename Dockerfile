FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY LostPeople.sln .
COPY src/Domain/*.csproj src/Domain/
COPY src/Application/*.csproj src/Application/
COPY src/Infrastructure/*.csproj src/Infrastructure/
COPY src/Web/*.csproj src/Web/
RUN dotnet restore
COPY . .
RUN dotnet publish src/Web/LostPeople.Web.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 80
EXPOSE 443
COPY --from=build /app .
ENTRYPOINT ["dotnet", "LostPeople.Web.dll"]
