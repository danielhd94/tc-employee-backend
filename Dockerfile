# Usa la imagen oficial de .NET Core SDK 3.1 como base para la etapa de construcción
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build

# Establece el directorio de trabajo en el contenedor
WORKDIR /app

# Copia solo los archivos .csproj para cada proyecto y restaura las dependencias
COPY ./WebUser/WebUser.csproj ./WebUser/
COPY ./WebUser.SRV/WebUser.SRV.csproj ./WebUser.SRV/

# Crea la carpeta Photos si no existe
RUN mkdir -p ./Photos

# Copia el contenido de la carpeta Photos si existe, si no, no hará nada
COPY ./WebUser/Photos/* ./Photos/ 2>/dev/null || :

# Restaura las dependencias para cada proyecto desde sus directorios
RUN dotnet restore ./WebUser/WebUser.csproj
RUN dotnet restore ./WebUser.SRV/WebUser.SRV.csproj

# Copia el resto de los archivos del proyecto
COPY . .

# Publica la aplicación en carpetas de salida específicas
RUN dotnet publish ./WebUser/WebUser.csproj -c Release -o /app/WebUser/out \
    && dotnet publish ./WebUser.SRV/WebUser.SRV.csproj -c Release -o /app/WebUser.SRV/out

# Usa una imagen más ligera de .NET Core para la ejecución
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS runtime
WORKDIR /app

# Copia las aplicaciones publicadas del contenedor de construcción
COPY --from=build /app/WebUser/out ./WebUser/
COPY --from=build /app/WebUser.SRV/out ./WebUser.SRV/
COPY --from=build /app/Photos ./Photos/

# Expone el puerto 5002 para tráfico HTTP
EXPOSE 5003

# Comando para iniciar la aplicación .NET Core
CMD ["dotnet", "WebUser/WebUser.dll"]