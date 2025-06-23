FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Fiap.Web.Aluno/Fiap.Web.Aluno.csproj", "Fiap.Web.Aluno/"]
RUN dotnet restore "Fiap.Web.Aluno/Fiap.Web.Aluno.csproj"
COPY . .
WORKDIR "/src/Fiap.Web.Aluno"
RUN dotnet build "./Fiap.Web.Aluno.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Fiap.Web.Aluno.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Fiap.Web.Aluno.dll"]
