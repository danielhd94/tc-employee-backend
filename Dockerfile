# Usa la imagen oficial de .NET Core 3.1 como base
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Establece el directorio de trabajo en el contenedor
WORKDIR /app

# Copia el archivo del proyecto y restaura las dependencias
COPY ./WebUser/WebUser.csproj ./WebUser/
COPY ./WebUser.SRV/WebUser.SRV.csproj ./WebUser.SRV/

RUN dotnet restore ./WebUser/WebUser.csproj
RUN dotnet restore ./WebUser.SRV/WebUser.SRV.csproj

# Copia el resto de los archivos y construye la aplicación
COPY ./WebUser ./WebUser/
COPY ./WebUser.SRV ./WebUser.SRV/

RUN dotnet publish -c Release -o /app/WebUser/out ./WebUser/WebUser.csproj
RUN dotnet publish -c Release -o /app/WebUser.SRV/out ./WebUser.SRV/WebUser.SRV.csproj

# Publica la aplicación en un contenedor ligero de .NET Core
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app
COPY --from=build /app/WebUser/out ./WebUser/
COPY --from=build /app/WebUser.SRV/out ./WebUser.SRV/

# Expone el puerto 80 para tráfico HTTP
EXPOSE 80

# Comando para iniciar la aplicación .NET Core
ENTRYPOINT ["dotnet", "WebUser.dll"]
