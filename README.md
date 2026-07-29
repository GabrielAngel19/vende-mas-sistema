# VendeMás

Sistema de punto de venta y tienda virtual compuesto por:

- `Backend/VendeMas.Api`: API REST con ASP.NET Core 8 y C#.
- `Frontend`: aplicación React 18 creada con Vite.

El frontend consulta productos y sucursales desde la API. Cuando el punto de
venta registra un cobro, el backend valida y descuenta el inventario; React
vuelve a consultar los datos y actualiza el catálogo público.

## Requisitos

- [.NET SDK 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- Node.js 18 o superior

## Ejecutar el backend C#

```powershell
dotnet restore Backend/VendeMas.sln
dotnet run --project Backend/VendeMas.Api
```

API: `http://localhost:5080`  
Swagger: `http://localhost:5080/swagger`

## Ejecutar el frontend React

En otra terminal:

```powershell
cd Frontend
npm install
npm run dev
```

Abrir `http://localhost:5173`.

Durante el desarrollo, Vite redirige las solicitudes `/api` a
`http://localhost:5080`. También puedes copiar `.env.example` como `.env` y
cambiar `VITE_API_URL` cuando el backend esté en otro servidor.

## Endpoints principales

| Método | Endpoint | Función |
| --- | --- | --- |
| GET | `/api/products` | Catálogo y disponibilidad por sucursal |
| GET | `/api/products/{id}` | Detalle de un producto |
| GET | `/api/stores` | Sucursales y coordenadas |
| POST | `/api/sales` | Registrar venta y descontar inventario |
| GET | `/api/dashboard` | Indicadores del negocio |

## Estructura

```text
Backend/
  VendeMas.sln
  VendeMas.Api/
    Controllers/
    Models/
    Services/
    Program.cs
Frontend/
  src/
    components/
    services/
    views/
    App.jsx
    main.jsx
```

Actualmente la información se mantiene en memoria. El siguiente paso es agregar
Entity Framework Core con SQLite para operación local y SQL Server para la
sincronización central planteada en el proyecto.
