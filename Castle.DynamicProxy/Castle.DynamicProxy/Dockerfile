#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["Castle.DynamicProxy/Castle.DynamicProxy.csproj", "Castle.DynamicProxy/"]
RUN dotnet restore "Castle.DynamicProxy/Castle.DynamicProxy.csproj"
COPY . .
WORKDIR "/src/Castle.DynamicProxy"
RUN dotnet build "Castle.DynamicProxy.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Castle.DynamicProxy.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Castle.DynamicProxy.dll"]