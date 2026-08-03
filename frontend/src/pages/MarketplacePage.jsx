import { ArrowRight, MapPin, Search, Store } from "lucide-react";
import { useEffect, useMemo, useState } from "react";
import { Link } from "react-router-dom";
import { getProducts } from "../api/products.js";
import { getStores } from "../api/stores.js";
import ProductCard from "../components/storefront/ProductCard.jsx";

function MarketplacePage() {
  const [products, setProducts] = useState([]);
  const [stores, setStores] = useState([]);
  const [search, setSearch] = useState("");
  const [category, setCategory] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    const controller = new AbortController();
    async function load() {
      setLoading(true);
      setError("");
      try {
        const [productData, storeData] = await Promise.all([
          getProducts({ search, category, availableOnly: true }, { signal: controller.signal }),
          getStores({}, { signal: controller.signal }),
        ]);
        setProducts(Array.isArray(productData) ? productData : []);
        setStores(Array.isArray(storeData) ? storeData : []);
      } catch (requestError) {
        if (requestError.name !== "AbortError") setError(requestError.message);
      } finally {
        setLoading(false);
      }
    }
    const timer = setTimeout(load, 250);
    return () => {
      clearTimeout(timer);
      controller.abort();
    };
  }, [search, category]);

  const storeById = useMemo(
    () => new Map(stores.map((store) => [store.id, store])),
    [stores],
  );
  const categories = useMemo(
    () => [...new Set(products.map((product) => product.category))].sort(),
    [products],
  );

  return (
    <main>
      <section className="sf-hero">
        <div className="sf-container sf-hero-grid">
          <div>
            <p className="sf-eyebrow">Encuentra productos cerca de ti</p>
            <h1>Lo que buscas, en una tienda de tu ciudad.</h1>
            <p className="sf-hero-text">
              Compara productos, conoce dónde están disponibles y compra en
              negocios registrados en VendeMás.
            </p>
            <label className="sf-search">
              <Search size={21} />
              <input
                onChange={(event) => setSearch(event.target.value)}
                placeholder="Busca un producto, marca o categoría"
                value={search}
              />
              <button type="button">Buscar</button>
            </label>
            <div className="sf-trust-row">
              <span><MapPin size={16} /> Ubicación de tiendas</span>
              <span><Store size={16} /> Negocios locales</span>
            </div>
          </div>
          <aside className="sf-hero-card">
            <span><Store size={35} /></span>
            <p>Tiendas registradas</p>
            <strong>{stores.length}</strong>
            <Link to="/tiendas">Ver todas <ArrowRight size={16} /></Link>
          </aside>
        </div>
      </section>

      <section className="sf-container sf-section">
        <div className="sf-section-heading">
          <div>
            <p className="sf-eyebrow">Catálogo local</p>
            <h2>Productos disponibles</h2>
          </div>
          <span>{products.length} resultados</span>
        </div>

        <div className="sf-categories" aria-label="Categorías">
          <button className={!category ? "active" : ""} onClick={() => setCategory("")} type="button">
            Todos
          </button>
          {categories.map((item) => (
            <button
              className={category === item ? "active" : ""}
              key={item}
              onClick={() => setCategory(item)}
              type="button"
            >
              {item}
            </button>
          ))}
        </div>

        {error && <p className="sf-alert">{error}</p>}
        {loading ? (
          <div className="sf-loading">Cargando productos…</div>
        ) : products.length ? (
          <div className="sf-product-grid">
            {products.map((product) => (
              <ProductCard
                key={product.id}
                product={product}
                store={storeById.get(product.storeId)}
              />
            ))}
          </div>
        ) : (
          <div className="sf-empty"><Search size={34} /><h3>No encontramos productos</h3><p>Prueba otra búsqueda o categoría.</p></div>
        )}
      </section>
    </main>
  );
}

export default MarketplacePage;
