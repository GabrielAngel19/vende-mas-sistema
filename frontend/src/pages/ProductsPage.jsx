import { useEffect, useMemo, useState } from "react";
import {
  createProduct,
  deactivateProduct,
  getAdminProducts,
  updateProduct,
} from "../api/admin.js";
import { getStoreBySlug } from "../api/stores.js";
import { useAuth } from "../auth/AuthContext.jsx";
import "./ProductsPage.css";

const COMMON_CATEGORIES = [
  "Abarrotes",
  "Alimentos",
  "Bebidas",
  "Belleza",
  "Electrónica",
  "Farmacia",
  "Hogar",
  "Papelería",
  "Ropa",
  "Servicios",
];

const EMPTY_FORM = {
  name: "",
  category: "",
  brand: "",
  description: "",
  code: "",
  imageUrl: "",
  price: "",
  stock: "0",
  unit: "pieza",
  branchId: "",
  isActive: true,
};

const money = new Intl.NumberFormat("es-MX", {
  style: "currency",
  currency: "MXN",
});

function ProductsPage() {
  const { activeStore } = useAuth();
  const [products, setProducts] = useState([]);
  const [branches, setBranches] = useState([]);
  const [search, setSearch] = useState("");
  const [form, setForm] = useState(EMPTY_FORM);
  const [editingId, setEditingId] = useState(null);
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [message, setMessage] = useState(null);

  const storeId = activeStore?.storeId;
  const storeSlug = activeStore?.storeSlug;

  async function loadData(signal, { clearMessage = true } = {}) {
    if (!storeId || !storeSlug) {
      setLoading(false);
      return;
    }

    setLoading(true);
    if (clearMessage) setMessage(null);

    try {
      const [productData, storeData] = await Promise.all([
        getAdminProducts(storeId, { signal }),
        getStoreBySlug(storeSlug, { signal }),
      ]);
      setProducts(productData);
      setBranches(storeData.branches ?? []);
      setForm((current) => ({
        ...current,
        branchId: current.branchId || String(storeData.branches?.[0]?.id ?? ""),
      }));
    } catch (error) {
      if (error.name !== "AbortError") {
        setMessage({ type: "error", text: error.message });
      }
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    const controller = new AbortController();
    loadData(controller.signal);
    return () => controller.abort();
  }, [storeId, storeSlug]);

  const categories = useMemo(
    () =>
      [...new Set([
        ...COMMON_CATEGORIES,
        ...products.map((product) => product.category).filter(Boolean),
      ])].sort((left, right) => left.localeCompare(right, "es")),
    [products],
  );

  const visibleProducts = useMemo(() => {
    const term = search.trim().toLocaleLowerCase("es");
    if (!term) return products;

    return products.filter((product) =>
      [product.name, product.category, product.brand, product.code]
        .filter(Boolean)
        .some((value) => value.toLocaleLowerCase("es").includes(term)),
    );
  }, [products, search]);

  function updateField(event) {
    const { name, value } = event.target;
    setForm((current) => ({ ...current, [name]: value }));
  }

  function openCreateForm() {
    setEditingId(null);
    setForm({
      ...EMPTY_FORM,
      branchId: String(branches[0]?.id ?? ""),
    });
    setMessage(null);
    setShowForm(true);
  }

  function openEditForm(product) {
    setEditingId(product.id);
    setForm({
      name: product.name ?? "",
      category: product.category ?? "",
      brand: product.brand ?? "",
      description: product.description ?? "",
      code: product.code ?? "",
      imageUrl: product.imageUrl ?? "",
      price: String(product.price ?? ""),
      stock: String(product.stock ?? 0),
      unit: product.unit ?? "pieza",
      branchId: String(branches[0]?.id ?? ""),
      isActive: product.isActive,
    });
    setMessage(null);
    setShowForm(true);
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setSaving(true);
    setMessage(null);

    try {
      if (editingId) {
        await updateProduct(editingId, {
          name: form.name.trim(),
          category: form.category,
          brand: form.brand.trim() || null,
          description: form.description.trim() || null,
          code: form.code.trim() || null,
          imageUrl: form.imageUrl.trim() || null,
          price: Number(form.price),
          unit: form.unit.trim(),
          isActive: form.isActive,
        });
      } else {
        await createProduct({
          storeId,
          branchId: Number(form.branchId),
          name: form.name.trim(),
          category: form.category,
          brand: form.brand.trim() || null,
          description: form.description.trim() || null,
          code: form.code.trim() || null,
          imageUrl: form.imageUrl.trim() || null,
          price: Number(form.price),
          stock: Number(form.stock),
          unit: form.unit.trim(),
        });
      }

      setShowForm(false);
      setMessage({
        type: "success",
        text: editingId ? "Producto actualizado." : "Producto creado.",
      });
      await loadData(undefined, { clearMessage: false });
    } catch (error) {
      setMessage({ type: "error", text: error.message });
    } finally {
      setSaving(false);
    }
  }

  async function handleStatusChange(product) {
    const nextActive = !product.isActive;
    setMessage(null);

    try {
      if (nextActive) {
        await updateProduct(product.id, {
          name: product.name,
          category: product.category,
          brand: product.brand,
          description: product.description,
          code: product.code,
          imageUrl: product.imageUrl,
          price: product.price,
          unit: product.unit,
          isActive: true,
        });
      } else {
        await deactivateProduct(product.id);
      }

      setMessage({
        type: "success",
        text: nextActive ? "Producto activado." : "Producto desactivado.",
      });
      await loadData(undefined, { clearMessage: false });
    } catch (error) {
      setMessage({ type: "error", text: error.message });
    }
  }

  if (!activeStore) {
    return (
      <main className="admin-products">
        <div className="admin-products__notice admin-products__notice--error">
          Tu usuario no tiene una tienda activa asignada.
        </div>
      </main>
    );
  }

  return (
    <main className="admin-products">
      <header className="admin-products__header">
        <div>
          <p className="admin-products__eyebrow">Catálogo de {activeStore.storeName}</p>
          <h1>Productos</h1>
          <p className="admin-products__subtitle">
            Crea, edita y controla qué productos aparecen en tu tienda.
          </p>
        </div>
        <button
          className="admin-products__button admin-products__button--primary"
          onClick={openCreateForm}
          type="button"
        >
          Nuevo producto
        </button>
      </header>

      {message && (
        <div className={`admin-products__notice admin-products__notice--${message.type}`}>
          {message.text}
        </div>
      )}

      {showForm && (
        <section className="admin-products__panel">
          <h2>{editingId ? "Editar producto" : "Registrar producto"}</h2>
          <form onSubmit={handleSubmit}>
            <div className="admin-products__form-grid">
              <label className="admin-products__field">
                Nombre
                <input name="name" onChange={updateField} required value={form.name} />
              </label>
              <label className="admin-products__field">
                Categoría
                <select name="category" onChange={updateField} required value={form.category}>
                  <option value="">Selecciona una categoría</option>
                  {categories.map((category) => (
                    <option key={category} value={category}>{category}</option>
                  ))}
                </select>
              </label>
              <label className="admin-products__field">
                Marca
                <input name="brand" onChange={updateField} value={form.brand} />
              </label>
              <label className="admin-products__field">
                Código o SKU
                <input name="code" onChange={updateField} value={form.code} />
              </label>
              <label className="admin-products__field">
                Precio
                <input min="0" name="price" onChange={updateField} required step="0.01" type="number" value={form.price} />
              </label>
              <label className="admin-products__field">
                Unidad
                <input name="unit" onChange={updateField} required value={form.unit} />
              </label>
              {!editingId && (
                <>
                  <label className="admin-products__field">
                    Sucursal inicial
                    <select name="branchId" onChange={updateField} required value={form.branchId}>
                      <option value="">Selecciona una sucursal</option>
                      {branches.map((branch) => (
                        <option key={branch.id} value={branch.id}>{branch.name}</option>
                      ))}
                    </select>
                  </label>
                  <label className="admin-products__field">
                    Existencia inicial
                    <input min="0" name="stock" onChange={updateField} required type="number" value={form.stock} />
                  </label>
                </>
              )}
              <label className="admin-products__field admin-products__field--wide">
                URL de imagen
                <input name="imageUrl" onChange={updateField} type="url" value={form.imageUrl} />
              </label>
              <label className="admin-products__field admin-products__field--wide">
                Descripción
                <textarea name="description" onChange={updateField} value={form.description} />
              </label>
            </div>
            {!branches.length && !editingId && (
              <p className="admin-products__notice admin-products__notice--error">
                Primero registra una sucursal para poder crear existencias.
              </p>
            )}
            <div className="admin-products__form-actions">
              <button className="admin-products__button" onClick={() => setShowForm(false)} type="button">
                Cancelar
              </button>
              <button
                className="admin-products__button admin-products__button--primary"
                disabled={saving || (!editingId && !branches.length)}
                type="submit"
              >
                {saving ? "Guardando…" : "Guardar producto"}
              </button>
            </div>
          </form>
        </section>
      )}

      <section className="admin-products__toolbar">
        <label className="admin-products__field">
          Buscar en el catálogo
          <input
            className="admin-products__search"
            onChange={(event) => setSearch(event.target.value)}
            placeholder="Nombre, categoría, marca o código"
            type="search"
            value={search}
          />
        </label>
        <strong>{visibleProducts.length} productos</strong>
      </section>

      <section className="admin-products__grid">
        {loading && <p className="admin-products__empty">Cargando productos…</p>}
        {!loading && !visibleProducts.length && (
          <p className="admin-products__empty">No encontramos productos para mostrar.</p>
        )}
        {!loading && visibleProducts.map((product) => (
          <article className="admin-products__card" key={product.id}>
            <div className="admin-products__badges">
              <span className="admin-products__badge">{product.category}</span>
              <span className={`admin-products__badge${product.isActive ? "" : " admin-products__badge--inactive"}`}>
                {product.isActive ? "Activo" : "Inactivo"}
              </span>
            </div>
            <div>
              <h3>{product.name}</h3>
              <p className="admin-products__muted">
                {[product.brand, product.code].filter(Boolean).join(" · ") || "Sin marca ni código"}
              </p>
            </div>
            <div className="admin-products__price">{money.format(product.price)}</div>
            <p>{product.stock} {product.unit} disponibles</p>
            <div className="admin-products__product-actions">
              <button className="admin-products__button" onClick={() => openEditForm(product)} type="button">
                Editar
              </button>
              <button
                className={`admin-products__button${product.isActive ? " admin-products__button--danger" : ""}`}
                onClick={() => handleStatusChange(product)}
                type="button"
              >
                {product.isActive ? "Desactivar" : "Activar"}
              </button>
            </div>
          </article>
        ))}
      </section>
    </main>
  );
}

export default ProductsPage;
