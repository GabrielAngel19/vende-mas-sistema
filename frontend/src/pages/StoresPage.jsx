import { ArrowRight, MapPin, Search, Store } from "lucide-react";
import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { getStores } from "../api/stores.js";

function StoresPage() {
  const [stores, setStores] = useState([]);
  const [search, setSearch] = useState("");
  const [error, setError] = useState("");

  useEffect(() => {
    const timer = setTimeout(async () => {
      try {
        setStores(await getStores({ search }));
        setError("");
      } catch (requestError) {
        setError(requestError.message);
      }
    }, 250);
    return () => clearTimeout(timer);
  }, [search]);

  return (
    <main className="sf-container sf-page">
      <div className="sf-page-heading">
        <p className="sf-eyebrow">Directorio local</p>
        <h1>Tiendas en VendeMás</h1>
        <p>Descubre negocios y consulta los productos que tienen disponibles.</p>
      </div>
      <label className="sf-search sf-search-compact">
        <Search size={20} />
        <input onChange={(event) => setSearch(event.target.value)} placeholder="Buscar tienda" value={search} />
      </label>
      {error && <p className="sf-alert">{error}</p>}
      <div className="sf-store-grid">
        {stores.map((store) => (
          <article className="sf-store-card" key={store.id}>
            <div className="sf-store-logo">
              {store.logoUrl ? <img alt="" src={store.logoUrl} /> : <Store size={32} />}
            </div>
            <div>
              <h2>{store.name}</h2>
              <p>{store.description || "Tienda local registrada en VendeMás."}</p>
              <span><MapPin size={15} /> {[store.city, store.state].filter(Boolean).join(", ") || "Ubicación por confirmar"}</span>
              <small>{store.productCount} productos</small>
            </div>
            <Link to={`/tiendas/${store.slug}`}>Visitar tienda <ArrowRight size={16} /></Link>
          </article>
        ))}
      </div>
    </main>
  );
}

export default StoresPage;
