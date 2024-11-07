#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 80

ENV ASPNETCORE_URLS=http://+:80;
ENV ASPNETCORE_ENVIRONMENT=Production
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
ENV LANG=pt_BR.UTF-8
ENV LANGUAGE=pt_BR.UTF-8
ENV LC_ALL=pt_BR.UTF-8

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["gerenciadorConsultasPICS/gerenciadorConsultasPICS.csproj", "gerenciadorConsultasPICS/"]
RUN dotnet restore "./gerenciadorConsultasPICS/./gerenciadorConsultasPICS.csproj"
COPY . .
WORKDIR "/src/gerenciadorConsultasPICS"
RUN dotnet build "./gerenciadorConsultasPICS.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./gerenciadorConsultasPICS.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "gerenciadorConsultasPICS.dll"]