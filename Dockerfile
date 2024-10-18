# Etapa de construcción
FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src

# Copia los archivos csproj y restaura las dependencias
COPY ["WebUser/WebUser.csproj", "WebUser/"]
COPY ["WebUser.SRV/WebUser.SRV.csproj", "WebUser.SRV/"]
RUN dotnet restore "WebUser/WebUser.csproj"
RUN dotnet restore "WebUser.SRV/WebUser.SRV.csproj"

# Copia el resto del código
COPY . .

# Compila y publica la aplicación
RUN dotnet publish "WebUser/WebUser.csproj" -c Release -o /app/publish

# Etapa final
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Crea un directorio para las fotos
RUN mkdir -p ./Photos

# Expone el puerto 5003 para tráfico HTTP
EXPOSE 5003

# Comando para iniciar la aplicación
ENTRYPOINT ["dotnet", "WebUser.dll"]