# VendeMás Backend

API REST de punto de venta construida con ASP.NET Core 10, Entity Framework
Core y SQL Server.

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

La conexión predeterminada utiliza SQL Server LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=VendeMasDb;Trusted_Connection=True
```

Para producción, define la cadena sin escribir credenciales en el repositorio:

```powershell
$env:ConnectionStrings__DefaultConnection = "TU_CADENA_DE_PRODUCCION"
```

## Crear la base de datos

Instala la herramienta de EF Core si aún no la tienes:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.10
```

Crea y aplica la migración:

```powershell
dotnet ef migrations add InitialCreate --project Backend.csproj
dotnet ef database update --project Backend.csproj
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
