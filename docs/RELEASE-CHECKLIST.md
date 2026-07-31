# Lista de preparación y entrega

## 1. Evidencia final

- [ ] Documentación actualizada con los PR #5 y #6.
- [ ] Sprint 3.2 y GitHub Actions documentados.
- [ ] Seis capturas disponibles en `docs/evidence/`.
- [ ] Ninguna captura contiene contraseñas o cadenas de conexión.
- [ ] Backend y frontend compilan localmente.
- [ ] Pull Request de evidencia fusionado a `develop` con CI exitoso.

## 2. Captura pendiente de venta e inventario

Con MariaDB, backend y frontend activos, completar una venta desde el POS. En
HeidiSQL ejecutar las siguientes consultas:

```sql
SELECT Id, PaymentMethod, Subtotal, Tax, Total, Status, CreatedAtUtc
FROM Sales
ORDER BY Id DESC
LIMIT 5;

SELECT Id, ProductId, SaleId, Type, QuantityChange, PreviousStock,
       NewStock, Reason, CreatedAtUtc
FROM InventoryMovements
ORDER BY Id DESC
LIMIT 10;
```

Tomar una captura que muestre la venta finalizada y los resultados de ambas
consultas, sin contraseñas ni cadenas de conexión. Guardarla como
`docs/evidence/05-sale-and-inventory.png` y comprobarla:

```powershell
Test-Path .\docs\evidence\05-sale-and-inventory.png
```

El resultado debe ser `True`.

## 3. Protección de ramas

Crear un ruleset activo para `main` y `develop` que impida eliminaciones y
force push, exija Pull Request y requiera `Backend build` y `Frontend build`.
No exigir aprobaciones ajenas mientras el repositorio tenga un solo autor.

## 4. Reconciliación inicial de `main`

La rama `main` contiene un commit antiguo que no pertenece al historial actual
de `develop`. La primera entrega debe reconciliar ambos historiales sin volver a
introducir el scaffold anterior.

Desde un árbol limpio y con `develop` actualizado:

```powershell
git fetch origin
git switch develop
git pull --ff-only origin develop
git status
git switch -c release/v1.0.0
git merge --no-ff -s ours origin/main `
  -m "chore(release): reconciliar historial de main con develop"
git diff --exit-code develop..HEAD
dotnet build .\Backend\Backend.csproj --configuration Release
npm ci --prefix .\frontend
npm run build --prefix .\frontend
git push -u origin release/v1.0.0
```

El comando `git diff --exit-code develop..HEAD` no debe imprimir diferencias.
La estrategia `ours` se utiliza solo en esta reconciliación inicial: registra el
historial antiguo de `main`, pero conserva exactamente los archivos validados de
`develop`.

Abrir un Pull Request de `release/v1.0.0` hacia `main`, esperar ambos controles
de CI y usar **Merge pull request**.

## 5. Verificación posterior

```powershell
git switch main
git pull --ff-only origin main
git switch develop
git pull --ff-only origin develop
git status
git log --oneline --decorate --graph -15
```

Crear en GitHub la etiqueta `v1.0.0` desde el commit final de `main` solo cuando
la aplicación y la documentación estén verificadas.

## 6. Sincronización posterior con `develop`

El merge de la entrega agrega a `main` un commit de versión que todavía no
existe en `develop`. Si el desarrollo continúa, crear la siguiente rama de
trabajo desde `main` actualizado y abrir su Pull Request hacia `develop`. Ese PR
debe incluir un cambio real, por ejemplo el `CHANGELOG.md` de la versión, para
incorporar el historial de la entrega sin hacer push directo a `develop`.

```powershell
git switch main
git pull --ff-only origin main
git switch -c chore/sync-v1-to-develop
```

Después de agregar y confirmar `CHANGELOG.md`, publicar la rama y abrir el PR:

```powershell
git add CHANGELOG.md
git commit -m "docs(release): registrar versión 1.0.0"
git push -u origin chore/sync-v1-to-develop
```

## 7. Limpieza de ramas

Eliminar únicamente ramas ya fusionadas. Primero comprobarlas:

```powershell
git fetch --prune origin
git branch --merged develop
git branch -r --merged origin/develop
```

Conservar siempre `main` y `develop`. Las ramas no fusionadas deben revisarse o
cerrarse explícitamente antes de eliminarlas.
