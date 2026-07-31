# Evidencia y validación

Este documento relaciona los criterios de evaluación con elementos verificables
del repositorio y con las pruebas realizadas durante el desarrollo.

## Trazabilidad con la rúbrica

| Criterio | Evidencia verificable |
| --- | --- |
| Metodología | [METHODOLOGY.md](METHODOLOGY.md), ramas por sprint y Pull Requests |
| Arquitectura | [ARCHITECTURE.md](ARCHITECTURE.md), separación física de carpetas y servicios |
| Stack tecnológico | [TECH-STACK.md](TECH-STACK.md), `package.json` y `Backend.csproj` |
| Flujo Git | [GIT-WORKFLOW.md](GIT-WORKFLOW.md), PR #2, #4, #5 y #6 |
| Consistencia | README y documentos alineados con React, ASP.NET Core y MariaDB |

## Evidencia de seguimiento

### Backend y persistencia

- [PR #2 - API POS con persistencia MariaDB](https://github.com/GabrielAngel19/vende-mas-sistema/pull/2)
- Migración `InitialMariaDb` versionada en `Backend/Data/Migrations`.
- Tablas verificadas: `Products`, `Customers`, `Sales`, `SaleItems`,
  `InventoryMovements` y `__EFMigrationsHistory`.
- Producto creado desde Swagger con respuesta HTTP 201.
- Compilación de `Backend.csproj` completada correctamente.

### Frontend e integración

- [PR #4 - POS integrado con la API](https://github.com/GabrielAngel19/vende-mas-sistema/pull/4)
- Compilación de Vite completada correctamente.
- Productos de MariaDB mostrados en React mediante `GET /api/products`.
- Venta registrada desde el POS mediante `POST /api/sales`.
- Reducción de existencias y movimiento de inventario comprobados.

### Documentación e integración continua

- [PR #5 - documentación del proyecto](https://github.com/GabrielAngel19/vende-mas-sistema/pull/5)
- [PR #6 - controles de calidad](https://github.com/GabrielAngel19/vende-mas-sistema/pull/6)
- Workflow `.github/workflows/ci.yml` versionado.
- `Backend build` completado correctamente en GitHub Actions.
- `Frontend build` completado correctamente en GitHub Actions.

### Seguridad y repositorio

- La cadena de conexión local utiliza .NET User Secrets.
- `appsettings.json` no contiene contraseña.
- `.gitignore` excluye dependencias, compilaciones y archivos locales.
- PR incorrectos o duplicados se cerraron sin fusionar.

## Comandos reproducibles

Desde la raíz del repositorio:

```powershell
git status
dotnet build .\Backend\Backend.csproj
dotnet ef database update `
  --project .\Backend\Backend.csproj `
  --startup-project .\Backend\Backend.csproj
```

Desde `frontend`:

```powershell
npm install
npm run build
npm run dev
```

Con backend y frontend activos:

```powershell
Invoke-RestMethod http://localhost:5000/api/health
Invoke-RestMethod http://localhost:5173/api/products
```

## Lista de comprobación de una entrega

- [x] El backend compila sin errores.
- [x] El frontend compila sin errores.
- [x] La migración se aplica correctamente.
- [x] Swagger muestra los controladores.
- [x] React carga productos reales.
- [x] Una venta crea registros en `Sales` y `SaleItems`.
- [x] El inventario disminuye y registra un movimiento.
- [x] `git status` no muestra archivos generados o secretos.
- [x] La documentación coincide con el código entregado.
- [x] GitHub Actions valida backend y frontend.

## Evidencia visual

Las capturas están almacenadas en `docs/evidence/` sin contraseñas ni cadenas de
conexión visibles:

| Evidencia | Validación |
| --- | --- |
| [Compilación del backend](evidence/01-backend-build.png) | Restauración y compilación de `Backend.csproj` |
| [Tablas de MariaDB](evidence/02-mariadb-tables.png) | Esquema creado por la migración `InitialMariaDb` |
| [Producto creado en Swagger](evidence/03-swagger-product-created.png) | Respuesta HTTP 201 de `POST /api/products` |
| [Productos reales en React](evidence/04-react-products.png) | Consumo de `GET /api/products` desde el POS |
| [Venta e inventario](evidence/05-sale-and-inventory.png) | Venta persistida y reducción de existencias |
| [Controles de CI](evidence/06-ci-checks.png) | `Backend build` y `Frontend build` exitosos |

El historial de GitHub sigue siendo la fuente primaria para ramas, commits y
Pull Requests; las capturas funcionan como apoyo visual.
