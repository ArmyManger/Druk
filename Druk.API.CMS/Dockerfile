#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:3.1 AS build
WORKDIR /src
COPY ["Druk.API.CMS/Druk.API.CMS.csproj", "Druk.API.CMS/"]
COPY ["Druk.DataCache/Druk.DataCache.csproj", "Druk.DataCache/"]
COPY ["Druk.Log/Druk.Log.csproj", "Druk.Log/"]
COPY ["Druk.RabbitMQ/Druk.RabbitMQ.csproj", "Druk.RabbitMQ/"]
COPY ["Druk.Common/Druk.Common.csproj", "Druk.Common/"]
COPY ["Druk.DB/Druk.DB.csproj", "Druk.DB/"]
RUN dotnet restore "Druk.API.CMS/Druk.API.CMS.csproj"
COPY . .
WORKDIR "/src/Druk.API.CMS"
RUN dotnet build "Druk.API.CMS.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Druk.API.CMS.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Druk.API.CMS.dll"]