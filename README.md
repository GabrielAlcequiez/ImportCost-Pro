# ImportCost-Pro
## Autores
- Jose Antonio Rincon (2025-1426)
- Gabriel Enmanuel Alcequiez (2025-1062)

## Descripción del Proyecto
ImportCost-Pro es una aplicación web MVC desarrollada para gestionar y calcular costos de importación (Landed Cost). El sistema está estructurado utilizando una arquitectura en capas (Web, BusinessLogic y Database) y aplica patrones de diseño como el patrón Repository y Unit of Work.

## Tecnologías Usadas
- **Framework**: .NET 9.0 (ASP.NET Core MVC)
- **Lenguaje**: C# 13
- **ORM**: Entity Framework Core 9.0.2
- **Base de Datos**: SQL Server
- **Validación**: FluentValidation
- **Frontend**: HTML5, CSS3, JavaScript (Vistas de MVC)

## Requisitos Previos
Para poder compilar y ejecutar este proyecto, necesitas:
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) o superior.
- Una instancia de SQL Server (LocalDB o completa) en funcionamiento.

## Instrucciones para Correr el Proyecto

1. **Clonar el repositorio**
   ```bash
   git clone https://github.com/GabrielAlcequiez/ImportCost-Pro.git
   cd ImportCost-Pro
   ```

2. **Configurar la cadena de conexión**
   Abre el archivo `appsettings.json` o `appsettings.Development.json` dentro de la carpeta `ImportCostPro.Web`. Asegúrate de que la cadena de conexión `DefaultConnection` apunte a tu servidor SQL Server:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=.;Database=ImportCostPro;Trusted_Connection=True;TrustServerCertificate=true"
   }
   ```
   *Nota: Por defecto, está configurada para conectarse a una instancia local de SQL Server (`Server=.`).*

3. **Aplicar las migraciones (Base de Datos)**
   Si no tienes la herramienta de comandos de EF Core instalada globalmente, instálala primero:
   ```bash
   dotnet tool install --global dotnet-ef
   ```
   Luego, aplica las migraciones a tu base de datos desde la raíz del proyecto:
   ```bash
   dotnet ef database update --project ImportCostPro.Database --startup-project ImportCostPro.Web
   ```

4. **Ejecutar la aplicación**
   Finalmente, inicia la aplicación ejecutando el siguiente comando:
   ```bash
   dotnet run --project ImportCostPro.Web
   ```
   La consola mostrará la URL local en la que se está ejecutando el proyecto (por ejemplo, `http://localhost:5000` o `https://localhost:5001`). Abre esa URL en tu navegador web.
