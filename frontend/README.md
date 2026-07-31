# VendeMás POS

Interfaz de punto de venta construida con React y Vite a partir del sistema de
diseño **VendeMás Fidelity**.

## Funciones incluidas

- Catálogo adaptable a escritorio, tablet y móvil.
- Búsqueda y filtrado por categorías.
- Carrito con cantidades, eliminación y cálculo de IVA.
- Métodos de pago y confirmación de venta.
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
const response = await fetch("/api/productos");
const productos = await response.json();
```

Ejecuta el backend en ese puerto:

```bash
dotnet run --project Backend --urls http://localhost:5000
```

Los productos actuales son datos de demostración definidos en `src/App.jsx`.
