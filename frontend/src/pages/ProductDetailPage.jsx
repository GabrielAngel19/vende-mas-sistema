import { ArrowLeft, MapPin, Package, ShoppingBag, Store } from "lucide-react";
import { useEffect, useState } from "react";
import { Link, useParams } from "react-router-dom";
import { getCatalogProductById } from "../api/catalog.js";
import { useCart } from "../storefront/CartContext.jsx";

const currency = new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN" });

function toCartProduct(product, offer) {
  return {
    id: offer.productId,
    name: product.name,
    price: offer.price,
    stock: offer.stock,
    unit: offer.unit,
    imageUrl: product.imageUrl,
    storeId: offer.storeId,
    storeName: offer.storeName,
  };
}

function ProductDetailPage() {
  const { id } = useParams();
  const { addItem } = useCart();
  const [product, setProduct] = useState(null);
  const [selectedOfferId, setSelectedOfferId] = useState(null);
  const [error, setError] = useState("");

  useEffect(() => {
    let active = true;
    async function load() {
      try {
        const productData = await getCatalogProductById(id);
        if (active) {
          setProduct(productData);
          const firstOffer = productData.offers?.find((offer) => offer.isAvailable)
            ?? productData.offers?.[0];
          setSelectedOfferId(firstOffer?.productId ?? null);
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

  const selectedOffer = product.offers?.find(
    (offer) => offer.productId === selectedOfferId,
  ) ?? product.offers?.[0];

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
          <strong className="sf-detail-price">
            {selectedOffer ? currency.format(selectedOffer.price) : currency.format(product.minPrice)}
          </strong>
          <p>{product.description || "Consulta disponibilidad y ubicación antes de visitar la tienda."}</p>
          <div className="sf-stock-box">
            <span className={product.totalStock > 0 ? "is-available" : "is-unavailable"}>
              {product.totalStock > 0
                ? `${product.totalStock} unidades entre ${product.storeCount} tiendas`
                : "Producto agotado"}
            </span>
          </div>
          <button
            className="sf-primary-button"
            disabled={!selectedOffer?.isAvailable}
            onClick={() => addItem(toCartProduct(product, selectedOffer))}
            type="button"
          >
            <ShoppingBag size={18} /> Agregar al carrito
          </button>

          <h2>Tiendas que ofrecen este producto</h2>
          {product.offers?.map((offer) => {
            const location = [offer.city, offer.state].filter(Boolean).join(", ");
            const isSelected = offer.productId === selectedOffer?.productId;

            return (
              <div className="sf-sold-by" key={offer.productId}>
                <Store size={24} />
                <div>
                  <small>Vendido por</small>
                  <strong>
                    {offer.storeSlug ? (
                      <Link to={`/tiendas/${offer.storeSlug}`}>{offer.storeName}</Link>
                    ) : offer.storeName}
                  </strong>
                  {location && <span><MapPin size={14} /> {location}</span>}
                  <span>{currency.format(offer.price)} · {offer.stock} disponibles</span>
                </div>
                <button
                  className="sf-outline-button"
                  disabled={!offer.isAvailable}
                  onClick={() => setSelectedOfferId(offer.productId)}
                  type="button"
                >
                  {isSelected ? "Seleccionada" : "Elegir"}
                </button>
              </div>
            );
          })}
        </div>
      </section>
    </main>
  );
}

export default ProductDetailPage;
