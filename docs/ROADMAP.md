# Roadmap funcional

Este documento distingue el alcance implementado de las funciones planificadas.
Su propósito es evitar que los elementos visuales del POS se interpreten como
funcionalidad terminada antes de contar con código y pruebas.

## Alcance implementado

- Consulta de productos reales desde MariaDB.
- Búsqueda y filtro por categoría.
- Carrito con cantidades limitadas por existencias.
- Cálculo de subtotal, IVA y total.
- Registro de ventas con tarjeta, efectivo o transferencia.
- Reducción transaccional de inventario.
- API REST para productos, clientes, ventas e inventario.
- Migraciones, Swagger, manejo uniforme de errores y CI.

## Incrementos pendientes

| Sprint propuesto | Alcance | Criterio de aceptación |
| --- | --- | --- |
| Sprint 4.1 | Separar `App.jsx` y agregar navegación | Cada pantalla tiene componente y ruta propia |
| Sprint 4.2 | Administración de productos | Listar, crear, editar y desactivar productos desde React |
| Sprint 4.3 | Administración de clientes | Listar, crear, editar y seleccionar un cliente en la venta |
| Sprint 4.4 | Historial de ventas e inventario | Consultar ventas, partidas, existencias bajas y movimientos |
| Sprint 5.1 | Sucursales y ajustes | Selector respaldado por datos persistentes y configuración editable |
| Sprint 5.2 | Escáner de código | Buscar y agregar un producto por código de barras |
| Sprint 5.3 | Fecha y usuario dinámicos | Fecha real y datos del usuario autenticado |
| Sprint 6.1 | Pruebas automatizadas | Pruebas backend y frontend ejecutadas en CI |
| Sprint 7.1 | Autenticación y roles | Inicio de sesión y autorización para administrador y cajero |

## Prioridad para una versión académica

La versión académica actual puede cerrarse con el flujo de venta ya validado.
Los botones de funciones futuras deben permanecer deshabilitados, etiquetados
como próximos o implementarse en ramas separadas antes de presentarlos como
funcionales.

## Reglas para cada incremento

1. Crear la rama desde `develop` actualizado.
2. Implementar un solo alcance por rama.
3. Agregar pruebas cuando exista infraestructura de pruebas.
4. Validar backend, frontend y migraciones afectadas.
5. Actualizar este roadmap y la documentación relacionada.
6. Abrir Pull Request hacia `develop` y esperar el CI.
