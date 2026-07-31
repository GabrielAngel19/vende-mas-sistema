# Stack tecnológico

## Resumen

Las siguientes versiones corresponden al código versionado y al entorno local
verificado durante el desarrollo.

## Frontend

| Tecnología | Versión | Uso | Justificación |
| --- | --- :| --- | --- |
| JavaScript | ES Modules | Lenguaje del cliente | Integración nativa con navegador y ecosistema React |
| React | 18.3.1 | Interfaz del POS | Componentes reutilizables y estado declarativo |
| React DOM | 18.3.1 | Renderizado web | Adaptador oficial de React para navegador |
| Vite | 6.4.3 instalada; `^6.0.5` declarada | Desarrollo y compilación | Inicio rápido, HMR y proxy configurable |
| Plugin React para Vite | `^4.3.4` | Transformación JSX | Integración oficial entre React y Vite |
| Lucide React | `^0.468.0` | Iconografía | Iconos consistentes, ligeros y accesibles |

## Backend

| Tecnología | Versión | Uso | Justificación |
| --- | --- :| --- | --- |
| C# | Incluido en .NET 10 | Lenguaje del servidor | Tipado estático, asincronía y ecosistema empresarial |
| .NET SDK | 10.0.302 | Compilación local | SDK instalado y compatible con el objetivo del proyecto |
| ASP.NET Core | `net10.0` | API REST | Controladores, inyección de dependencias, CORS y middleware |
| Microsoft.AspNetCore.OpenApi | 10.0.10 | Documento OpenAPI | Describe los endpoints de la API |
| Entity Framework Core Design | 9.0.18 | Migraciones | Mantiene el esquema sincronizado con los modelos |
| Pomelo EF Core MySQL | 9.0.0 | Proveedor de datos | Conecta EF Core con MariaDB/MySQL |
| Swashbuckle Swagger UI | 10.2.3 | Exploración de la API | Permite probar endpoints desde el navegador |
| Microsoft.OpenApi | 2.7.5 | Modelos OpenAPI | Soporte para la descripción de la API |

El backend apunta a .NET 10 y utiliza la línea 9 de EF Core/Pomelo. Esta
combinación fue compilada y validada en el entorno del proyecto. Cualquier
actualización debe probarse en una rama independiente antes de cambiar estas
versiones.

## Base de datos y herramientas

| Tecnología | Versión verificada | Uso | Justificación |
| --- | --- :| --- | --- |
| MariaDB | 11.8.8 | Base relacional | Transacciones, claves foráneas e índices; disponible en Laragon |
| Laragon | 2026 v8.6.1 | Entorno local | Administración sencilla de servicios en Windows |
| HeidiSQL | 12.8.0.6908 | Inspección de datos | Visualiza tablas, relaciones y registros de MariaDB |
| Node.js | 24.18.0 | Runtime del frontend | Ejecuta npm y Vite |
| npm | 11.16.0 | Dependencias frontend | Instalación reproducible mediante `package-lock.json` |
| Git | 2.55.0.windows.2 | Control de versiones | Ramas, commits y trazabilidad |
| GitHub | Servicio web | Colaboración | Pull Requests, revisión e historial remoto |
| Swagger UI | Integrado al backend | Pruebas manuales | Verificación rápida de contratos HTTP |

## Criterios de selección

1. **Compatibilidad:** React consume JSON producido por ASP.NET Core sin una
   capa propietaria.
2. **Separación:** frontend, backend y base de datos tienen responsabilidades
   independientes.
3. **Persistencia relacional:** ventas, partidas, clientes e inventario requieren
   transacciones y relaciones consistentes.
4. **Productividad local:** Laragon y HeidiSQL simplifican la administración de
   MariaDB en Windows.
5. **Trazabilidad:** GitHub y las migraciones conservan el historial del código y
   del esquema.
6. **Seguridad:** User Secrets evita versionar credenciales de desarrollo.

## Verificación de versiones

```powershell
dotnet --version
node --version
npm --version
git --version
mariadb --version
dotnet list .\Backend\Backend.csproj package
```

Las versiones declaradas del frontend se consultan en `frontend/package.json` y
las del backend en `Backend/Backend.csproj`.
