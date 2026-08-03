import { MapPin, Package, Plus, Store } from "lucide-react";
import { Link } from "react-router-dom";
import { useCart } from "../../storefront/CartContext.jsx";

const currency = new Intl.NumberFormat("es-MX", {
  style: "currency",
  currency: "MXN",
});

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

function ProductCard({ product }) {
  const { addItem } = useCart();
  const bestOffer = product.offers?.find((offer) => offer.isAvailable) ?? product.offers?.[0];
  const location = [bestOffer?.city, bestOffer?.state].filter(Boolean).join(", ");
  const detailId = product.productId ?? bestOffer?.productId;

  return (
    <article className="sf-product-card">
      <Link className="sf-product-image" to={`/productos/${detailId}`}>
        {product.imageUrl ? (
          <img alt={product.name} loading="lazy" src={product.imageUrl} />
        ) : (
          <Package aria-hidden="true" size={48} />
        )}
        <span>{product.category}</span>
      </Link>

      <div className="sf-product-copy">
        <Link to={`/productos/${detailId}`}>
          <h3>{product.name}</h3>
        </Link>
        {product.brand && <p className="sf-product-brand">{product.brand}</p>}
        <strong className="sf-price">Desde {currency.format(product.minPrice)}</strong>
        <small>{product.storeCount} {product.storeCount === 1 ? "tienda" : "tiendas"}</small>

        <div className="sf-product-store">
          <Store size={15} />
          {bestOffer?.storeSlug ? (
            <Link to={`/tiendas/${bestOffer.storeSlug}`}>{bestOffer.storeName}</Link>
          ) : (
            <span>{bestOffer?.storeName || "Tienda VendeMás"}</span>
          )}
        </div>
        {location && <p className="sf-location"><MapPin size={14} /> {location}</p>}

        <div className="sf-card-footer">
          <span className={product.totalStock > 0 ? "is-available" : "is-unavailable"}>
            {product.totalStock > 0 ? `${product.totalStock} disponibles` : "Agotado"}
          </span>
          <button
            disabled={!bestOffer?.isAvailable}
            onClick={() => addItem(toCartProduct(product, bestOffer))}
            type="button"
          >
            <Plus size={17} /> Agregar
          </button>
        </div>
      </div>
    </article>
  );
}

export default ProductCard;
