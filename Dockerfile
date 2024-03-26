## school_management-backend Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
EXPOSE 80
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["school_management-backend.csproj", "./"]
RUN dotnet restore "school_management-backend.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "school_management-backend.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "school_management-backend.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "school_management_backend.dll"]
