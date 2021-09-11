FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["Phones.csproj", "."]
RUN dotnet restore "Phones.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "Phones.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Phones.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Phones.dll"]
