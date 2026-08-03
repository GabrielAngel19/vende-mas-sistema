import { MapPin, Package, Plus, Store } from "lucide-react";
import { Link } from "react-router-dom";
import { useCart } from "../../storefront/CartContext.jsx";

const currency = new Intl.NumberFormat("es-MX", {
  style: "currency",
  currency: "MXN",
});

function ProductCard({ product, store }) {
  const { addItem } = useCart();
  const location = [store?.city, store?.state].filter(Boolean).join(", ");

  return (
    <article className="sf-product-card">
      <Link className="sf-product-image" to={`/productos/${product.id}`}>
        {product.imageUrl ? (
          <img alt={product.name} loading="lazy" src={product.imageUrl} />
        ) : (
          <Package aria-hidden="true" size={48} />
        )}
        <span>{product.category}</span>
      </Link>

      <div className="sf-product-copy">
        <Link to={`/productos/${product.id}`}>
          <h3>{product.name}</h3>
        </Link>
        {product.brand && <p className="sf-product-brand">{product.brand}</p>}
        <strong className="sf-price">{currency.format(product.price)}</strong>
        <small>por {product.unit || "pieza"}</small>

        <div className="sf-product-store">
          <Store size={15} />
          {store?.slug ? (
            <Link to={`/tiendas/${store.slug}`}>{product.storeName || store.name}</Link>
          ) : (
            <span>{product.storeName || "Tienda VendeMás"}</span>
          )}
        </div>
        {location && <p className="sf-location"><MapPin size={14} /> {location}</p>}

        <div className="sf-card-footer">
          <span className={product.stock > 0 ? "is-available" : "is-unavailable"}>
            {product.stock > 0 ? `${product.stock} disponibles` : "Agotado"}
          </span>
          <button
            disabled={product.stock < 1}
            onClick={() => addItem(product)}
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
