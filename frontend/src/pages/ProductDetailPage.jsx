import { ArrowLeft, MapPin, Package, ShoppingBag, Store } from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getProductById } from "../api/products.js";
import { getStores } from "../api/stores.js";
import { useCart } from "../storefront/CartContext.jsx";

const currency = new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN" });

function ProductDetailPage() {
  const { id } = useParams();
  const { addItem } = useCart();
  const [product, setProduct] = useState(null);
  const [store, setStore] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;
    async function load() {
      try {
        const [productData, stores] = await Promise.all([getProductById(id), getStores()]);
        if (active) {
          setProduct(productData);
          setStore(stores.find((item) => item.id === productData.storeId));
        }
      } catch (requestError) {
        if (active) setError(requestError.message);
      }
    }
    load();
    return () => { active = false; };
  }, [id]);

  if (error) return <main className="sf-container sf-page"><p className="sf-alert">{error}</p></main>;
  if (!product) return <main className="sf-loading">Cargando producto…</main>;

  return (
    <main className="sf-container sf-page">
      <Link className="sf-back" to="/"><ArrowLeft size={17} /> Volver al catálogo</Link>
      <section className="sf-product-detail">
        <div className="sf-detail-image">
          {product.imageUrl ? <img alt={product.name} src={product.imageUrl} /> : <Package size={86} />}
        </div>
        <div className="sf-detail-copy">
          <p className="sf-eyebrow">{product.category}</p>
          <h1>{product.name}</h1>
          {product.brand && <p className="sf-detail-brand">Marca: {product.brand}</p>}
          <strong className="sf-detail-price">{currency.format(product.price)}</strong>
          <p>{product.description || "Consulta disponibilidad y ubicación antes de visitar la tienda."}</p>
          <div className="sf-stock-box">
            <span className={product.stock > 0 ? "is-available" : "is-unavailable"}>
              {product.stock > 0 ? `${product.stock} ${product.unit} disponibles` : "Producto agotado"}
            </span>
          </div>
          <button className="sf-primary-button" disabled={product.stock < 1} onClick={() => addItem(product)} type="button">
            <ShoppingBag size={18} /> Agregar al carrito
          </button>

          <div className="sf-sold-by">
            <Store size={24} />
            <div><small>Vendido por</small><strong>{product.storeName || store?.name || "Tienda VendeMás"}</strong>
              {store && <span><MapPin size={14} /> {[store.city, store.state].filter(Boolean).join(", ")}</span>}
            </div>
            {store?.slug && <Link to={`/tiendas/${store.slug}`}>Ver tienda</Link>}
          </div>
        </div>
      </section>
    </main>
  );
}

export default ProductDetailPage;
