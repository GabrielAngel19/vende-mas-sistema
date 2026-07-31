# VendeMás Backend

API REST de punto de venta construida con ASP.NET Core 10, Entity Framework
Core 9, Pomelo y MariaDB 11.8.
## Estructura

```text
Backend/
├── Controllers/   Endpoints HTTP
├── Data/          DbContext y configuración de EF Core
├── DTOs/          Contratos de entrada y salida
├── Middleware/    Respuestas de error uniformes
├── Models/        Entidades persistentes
└── Services/      Reglas de negocio
```

## Configuración local

La aplicación utiliza MariaDB en el puerto 3306. La contraseña no se guarda
en `appsettings.json`; para desarrollo se utiliza .NET User Secrets.

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" `
  "Server=localhost;Port=3306;Database=vendemasdb;User=Admin;Password=TU_CLAVE;" `
  --project .\Backend.csproj

## Crear la base de datos

Instala la herramienta de EF Core si aún no la tienes:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.18
```

Crea y aplica la migración:

```powershell
dotnet ef migrations add InitialMariaDb `
  --project .\Backend.csproj `
  --startup-project .\Backend.csproj `
  --output-dir Data\Migrations

dotnet ef database update `
  --project .\Backend.csproj `
  --startup-project .\Backend.csproj
```

## Ejecutar

```powershell
dotnet restore
dotnet run --urls http://localhost:5000
```

Documento OpenAPI en desarrollo:

```text
http://localhost:5000/openapi/v1.json
```

## Endpoints principales

| Método | Ruta | Función |
|---|---|---|
| GET/POST | `/api/products` | Consultar o crear productos |
| GET/PUT/DELETE | `/api/products/{id}` | Administrar un producto |
| GET/POST | `/api/customers` | Consultar o crear clientes |
| GET/PUT/DELETE | `/api/customers/{id}` | Administrar un cliente |
| GET | `/api/inventory` | Estado actual de existencias |
| GET | `/api/inventory/low-stock` | Productos con existencia baja |
| GET | `/api/inventory/movements` | Historial de movimientos |
| POST | `/api/inventory/{id}/adjustments` | Entrada o salida manual |
| GET/POST | `/api/sales` | Consultar o registrar ventas |
| GET | `/api/sales/{id}` | Consultar una venta |

Registrar una venta descuenta inventario dentro de una transacción. Si algún
producto no tiene existencias suficientes, la operación completa se cancela.
