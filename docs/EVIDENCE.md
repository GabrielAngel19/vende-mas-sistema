# Evidencia y validación

Este documento relaciona los criterios de evaluación con elementos verificables
del repositorio y con las pruebas realizadas durante el desarrollo.

## Trazabilidad con la rúbrica

| Criterio | Evidencia verificable |
| --- | --- |
| Metodología | [METHODOLOGY.md](METHODOLOGY.md), ramas por sprint y Pull Requests |
| Arquitectura | [ARCHITECTURE.md](ARCHITECTURE.md), separación física de carpetas y servicios |
| Stack tecnológico | [TECH-STACK.md](TECH-STACK.md), `package.json` y `Backend.csproj` |
| Flujo Git | [GIT-WORKFLOW.md](GIT-WORKFLOW.md), PR #2 y PR #4 |
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

- [ ] El backend compila sin errores.
- [ ] El frontend compila sin errores.
- [ ] La migración se aplica correctamente.
- [ ] Swagger muestra los controladores.
- [ ] React carga productos reales.
- [ ] Una venta crea registros en `Sales` y `SaleItems`.
- [ ] El inventario disminuye y registra un movimiento.
- [ ] `git status` no muestra archivos generados o secretos.
- [ ] La documentación coincide con el código entregado.

## Evidencia visual recomendada

Para una entrega académica se pueden anexar en `docs/evidence/` capturas sin
contraseñas ni datos sensibles con los siguientes nombres:

1. `01-backend-build.png`
2. `02-mariadb-tables.png`
3. `03-swagger-product-created.png`
4. `04-react-products.png`
5. `05-sale-and-inventory.png`
6. `06-pull-requests.png`

El historial de GitHub sigue siendo la fuente primaria para ramas, commits y
Pull Requests; las capturas funcionan como apoyo visual.
