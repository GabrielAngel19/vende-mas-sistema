import { Minus, Plus, ShoppingBag, Trash2 } from "lucide-react";
import { Link } from "react-router-dom";
import { useCart } from "../storefront/CartContext.jsx";

const currency = new Intl.NumberFormat("es-MX", { style: "currency", currency: "MXN" });

function CartPage() {
  const { items, subtotal, setQuantity, removeItem } = useCart();

  return (
    <main className="sf-container sf-page">
      <div className="sf-page-heading">
        <p className="sf-eyebrow">Tu selección</p>
        <h1>Carrito</h1>
        <p>Revisa los productos y la tienda que los vende.</p>
      </div>

      {!items.length ? (
        <div className="sf-empty"><ShoppingBag size={42} /><h2>Tu carrito está vacío</h2><p>Explora productos disponibles cerca de ti.</p><Link className="sf-primary-button" to="/">Ver productos</Link></div>
      ) : (
        <div className="sf-cart-layout">
          <section className="sf-cart-items">
            {items.map((item) => (
              <article key={item.id}>
                <div className="sf-cart-image">{item.imageUrl ? <img alt="" src={item.imageUrl} /> : <ShoppingBag size={24} />}</div>
                <div className="sf-cart-copy"><strong>{item.name}</strong><span>{item.storeName || "Tienda VendeMás"}</span><small>{currency.format(item.price)} por {item.unit}</small></div>
                <div className="sf-quantity">
                  <button onClick={() => setQuantity(item.id, item.quantity - 1)} type="button"><Minus size={15} /></button>
                  <span>{item.quantity}</span>
                  <button onClick={() => setQuantity(item.id, item.quantity + 1)} type="button"><Plus size={15} /></button>
                </div>
                <strong>{currency.format(item.price * item.quantity)}</strong>
                <button className="sf-remove" onClick={() => removeItem(item.id)} type="button"><Trash2 size={17} /></button>
              </article>
            ))}
          </section>
          <aside className="sf-cart-summary">
            <h2>Resumen</h2>
            <div><span>Subtotal</span><strong>{currency.format(subtotal)}</strong></div>
            <div><span>Envío</span><span>Por definir</span></div>
            <hr />
            <div className="sf-cart-total"><span>Total estimado</span><strong>{currency.format(subtotal)}</strong></div>
            <button disabled type="button">Pedidos en línea: siguiente sprint</button>
            <p>Por ahora usa VendeMás para localizar el producto y visitar la tienda.</p>
          </aside>
        </div>
      )}
    </main>
  );
}

export default CartPage;
