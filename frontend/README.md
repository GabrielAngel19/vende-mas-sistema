# VendeMás POS

Interfaz de punto de venta construida con React y Vite a partir del sistema de
diseño **VendeMás Fidelity**.

## Funciones incluidas

- Catálogo adaptable a escritorio, tablet y móvil.
- Búsqueda y filtrado por categorías.
- Carrito con cantidades, eliminación y cálculo de IVA.
- Métodos de pago y confirmación de venta.
- Navegación mediante React Router.
- Páginas independientes para venta, productos, ventas, clientes y ajustes.
- Productos reales obtenidos desde la API REST.
- Proxy de desarrollo para un backend C# en `http://localhost:5000`.

## Ejecutar

```bash
npm install
npm run dev
```

Abre `http://localhost:5173`.

## Conectar con ASP.NET Core

El archivo `vite.config.js` redirige las solicitudes que comienzan con `/api`
hacia `http://localhost:5000`.

Ejemplo desde React:

```js
const response = await fetch("/api/products");
const productos = await response.json();
```

Ejecuta el backend en ese puerto:

```bash
dotnet run --project ..\Backend\Backend.csproj
```

## Estructura principal

```text
src/
├── api/          Cliente HTTP por recurso
├── components/   Componentes compartidos
├── hooks/        Estado y acceso a datos reutilizable
├── layouts/      Distribución y navegación principal
├── pages/        Pantallas asociadas a rutas
└── App.jsx       Definición de rutas
```

La pantalla de venta conserva el flujo funcional. Los módulos administrativos
muestran su alcance planificado y se implementarán en ramas posteriores.
