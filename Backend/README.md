# VendeMás Backend

API REST del punto de venta VendeMás, construida con ASP.NET Core 10,
Entity Framework Core 9, Pomelo y MariaDB 11.8.

## Responsabilidades

- Exponer endpoints HTTP para productos, clientes, inventario y ventas.
- Validar los contratos de entrada mediante DTOs.
- Ejecutar reglas de negocio en servicios especializados.
- Persistir información y relaciones mediante Entity Framework Core.
- Registrar ventas y movimientos de inventario en una transacción.
- Responder errores con un formato uniforme basado en Problem Details.

## Estructura

```text
Backend/
├── Controllers/   Endpoints HTTP
├── Data/          DbContext, configuración y migraciones
├── DTOs/          Contratos de entrada y salida
├── Middleware/    Manejo uniforme de excepciones
├── Models/        Entidades persistentes
├── Services/      Reglas de negocio
├── Program.cs     Registro y canalización de la aplicación
└── Backend.csproj Dependencias y configuración del proyecto
```

Consulta [la arquitectura completa](../docs/ARCHITECTURE.md).

## Configuración local

La API utiliza MariaDB en el puerto 3306. La contraseña no se guarda en
`appsettings.json`; en desarrollo se utiliza .NET User Secrets.

Desde la carpeta `Backend`:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" `
  "Server=localhost;Port=3306;Database=vendemasdb;User=Admin;Password=TU_CLAVE;" `
  --project .\Backend.csproj
```

Para comprobar que el secreto existe sin imprimir su valor:

```powershell
dotnet user-secrets list --project .\Backend.csproj
```

## Migraciones

Instala la herramienta de EF Core si aún no está disponible:

```powershell
dotnet tool install --global dotnet-ef --version 9.0.18
```

La migración inicial ya está versionada. Para crear una migración después de
modificar los modelos:

```powershell
dotnet ef migrations add NombreDeLaMigracion `
  --project .\Backend.csproj `
  --startup-project .\Backend.csproj `
  --output-dir Data\Migrations
```

Para actualizar la base de datos:

```powershell
dotnet ef database update `
  --project .\Backend.csproj `
  --startup-project .\Backend.csproj
```

## Ejecutar y validar

```powershell
dotnet restore .\Backend.csproj
dotnet build .\Backend.csproj
dotnet run --project .\Backend.csproj
```

Servicios disponibles en desarrollo:

- API: `http://localhost:5000`
- Salud: `http://localhost:5000/api/health`
- OpenAPI: `http://localhost:5000/openapi/v1.json`
- Swagger UI: `http://localhost:5000/swagger/index.html`

## Endpoints principales

| Método | Ruta | Función |
| --- | --- | --- |
| GET / POST | `/api/products` | Consultar o crear productos |
| GET / PUT / DELETE | `/api/products/{id}` | Administrar un producto |
| GET / POST | `/api/customers` | Consultar o crear clientes |
| GET / PUT / DELETE | `/api/customers/{id}` | Administrar un cliente |
| GET | `/api/inventory` | Consultar existencias |
| GET | `/api/inventory/low-stock` | Consultar productos con existencia baja |
| GET | `/api/inventory/movements` | Consultar movimientos |
| POST | `/api/inventory/{id}/adjustments` | Registrar un ajuste manual |
| GET / POST | `/api/sales` | Consultar o registrar ventas |
| GET | `/api/sales/{id}` | Consultar una venta |

Registrar una venta descuenta inventario dentro de una transacción serializable.
Si algún producto no existe, está inactivo o no tiene existencias suficientes,
la operación completa se cancela.
