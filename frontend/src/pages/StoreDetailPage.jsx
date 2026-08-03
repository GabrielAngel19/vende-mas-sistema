import { ExternalLink, MapPin, Store } from "lucide-react";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getProducts } from "../api/products.js";
import { getStoreBySlug } from "../api/stores.js";
import ProductCard from "../components/storefront/ProductCard.jsx";

function mapUrl(branch) {
  const query = branch.latitude && branch.longitude
    ? `${branch.latitude},${branch.longitude}`
    : [branch.address, branch.city, branch.state].filter(Boolean).join(", ");
  return `https://www.google.com/maps/search/?api=1&query=${encodeURIComponent(query)}`;
}

function StoreDetailPage() {
  const { slug } = useParams();
  const [store, setStore] = useState(null);
  const [products, setProducts] = useState([]);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;
    async function load() {
      try {
        const storeData = await getStoreBySlug(slug);
        const productData = await getProducts({ storeId: storeData.id, availableOnly: true });
        if (active) {
          setStore(storeData);
          setProducts(productData);
        }
      } catch (requestError) {
        if (active) setError(requestError.message);
      }
    }
    load();
    return () => { active = false; };
  }, [slug]);

  if (error) return <main className="sf-container sf-page"><p className="sf-alert">{error}</p></main>;
  if (!store) return <main className="sf-loading">Cargando tienda…</main>;

  const primaryBranch = store.branches.find((branch) => branch.isPrimary) || store.branches[0];
  const summary = primaryBranch
    ? { city: primaryBranch.city, state: primaryBranch.state, slug: store.slug, name: store.name }
    : { slug: store.slug, name: store.name };

  return (
    <main>
      <section className="sf-store-hero">
        <div className="sf-container">
          <div className="sf-store-logo sf-store-logo-large">
            {store.logoUrl ? <img alt="" src={store.logoUrl} /> : <Store size={42} />}
          </div>
          <div>
            <p className="sf-eyebrow">Tienda verificada</p>
            <h1>{store.name}</h1>
            <p>{store.description || "Productos disponibles en esta tienda local."}</p>
          </div>
        </div>
      </section>

      <section className="sf-container sf-section">
        <div className="sf-branch-list">
          {store.branches.map((branch) => (
            <article key={branch.id}>
              <MapPin size={21} />
              <div><strong>{branch.name}</strong><span>{branch.address}, {branch.city}, {branch.state}</span></div>
              <a href={mapUrl(branch)} rel="noreferrer" target="_blank">Ver mapa <ExternalLink size={14} /></a>
            </article>
          ))}
        </div>
        <div className="sf-section-heading"><div><p className="sf-eyebrow">Catálogo</p><h2>Productos de {store.name}</h2></div><span>{products.length} disponibles</span></div>
        <div className="sf-product-grid">
          {products.map((product) => <ProductCard key={product.id} product={product} store={summary} />)}
        </div>
      </section>
    </main>
  );
}

export default StoreDetailPage;
