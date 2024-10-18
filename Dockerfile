# Usa la imagen oficial de .NET Core SDK 3.1 como base para la etapa de construcción
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Establece el directorio de trabajo en el contenedor
WORKDIR /app

# Copia solo los archivos .csproj para cada proyecto y restaura las dependencias
COPY ./WebUser/WebUser.csproj ./WebUser/
COPY ./WebUser.SRV/WebUser.SRV.csproj ./WebUser.SRV/
COPY ./WebUser/Photos/ ./Photos/

# Restaura las dependencias para cada proyecto desde sus directorios
RUN dotnet restore ./WebUser/WebUser.csproj
RUN dotnet restore ./WebUser.SRV/WebUser.SRV.csproj

# Copia el resto de los archivos del proyecto, incluyendo la carpeta Photos
COPY . .

# Publica la aplicación en carpetas de salida específicas
RUN dotnet publish ./WebUser/WebUser.csproj -c Release -o /app/WebUser/out \
    && dotnet publish ./WebUser.SRV/WebUser.SRV.csproj -c Release -o /app/WebUser.SRV/out

# Usa una imagen más ligera de .NET Core para la ejecución
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

# Copia las aplicaciones publicadas y la carpeta Photos del contenedor de construcción
COPY --from=build /app/WebUser/out ./WebUser/
COPY --from=build /app/WebUser.SRV/out ./WebUser.SRV/
COPY --from=build /app/WebUser/Photos/ ./Photos/

# Copia los archivos .env y .env.development
# COPY .env.Production .
# COPY .env.Development .

# Expone los puertos para tráfico HTTP
EXPOSE 5003

# Comando para iniciar la aplicación .NET Core
ENTRYPOINT ["dotnet", "WebUser/WebUser.dll"]