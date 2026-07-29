import { useMemo, useState } from "react";
import {
  Apple,
  Banknote,
  Barcode,
  Beef,
  Bell,
  Carrot,
  Check,
  CheckCircle2,
  ChevronDown,
  Coffee,
  Cookie,
  CreditCard,
  Croissant,
  CupSoda,
  Grid2X2,
  LayoutDashboard,
  Landmark,
  Menu,
  Milk,
  Minus,
  Package,
  Plus,
  ReceiptText,
  Search,
  Settings,
  ShoppingBasket,
  Store,
  Trash2,
  Users,
  Wheat,
  X,
} from "lucide-react";
import "./App.css";

const categories = [
  { id: "Todos", label: "Todos", icon: Grid2X2 },
  { id: "Panadería", label: "Panadería", icon: Croissant },
  { id: "Frutas", label: "Frutas", icon: Apple },
  { id: "Bebidas", label: "Bebidas", icon: CupSoda },
  { id: "Abarrotes", label: "Abarrotes", icon: Package },
  { id: "Lácteos", label: "Lácteos", icon: Milk },
];

const products = [
  {
    id: 1,
    name: "Pan artesanal",
    category: "Panadería",
    price: 45,
    stock: 18,
    unit: "pieza",
    color: "sage",
    icon: Wheat,
  },
  {
    id: 2,
    name: "Croissant mantequilla",
    category: "Panadería",
    price: 32,
    stock: 9,
    unit: "pieza",
    color: "sand",
    icon: Croissant,
  },
  {
    id: 3,
    name: "Manzana roja",
    category: "Frutas",
    price: 42,
    stock: 24,
    unit: "kg",
    color: "peach",
    icon: Apple,
  },
  {
    id: 4,
    name: "Zanahoria orgánica",
    category: "Frutas",
    price: 29,
    stock: 15,
    unit: "kg",
    color: "orange",
    icon: Carrot,
  },
  {
    id: 5,
    name: "Leche entera",
    category: "Lácteos",
    price: 31,
    stock: 20,
    unit: "litro",
    color: "blue",
    icon: Milk,
  },
  {
    id: 6,
    name: "Café de especialidad",
    category: "Bebidas",
    price: 168,
    stock: 12,
    unit: "bolsa",
    color: "coffee",
    icon: Coffee,
  },
  {
    id: 7,
    name: "Agua mineral",
    category: "Bebidas",
    price: 24,
    stock: 32,
    unit: "botella",
    color: "aqua",
    icon: CupSoda,
  },
  {
    id: 8,
    name: "Galletas de avena",
    category: "Abarrotes",
    price: 56,
    stock: 6,
    unit: "paquete",
    color: "sand",
    icon: Cookie,
  },
  {
    id: 9,
    name: "Carne para asar",
    category: "Abarrotes",
    price: 215,
    stock: 8,
    unit: "kg",
    color: "rose",
    icon: Beef,
  },
];

const money = new Intl.NumberFormat("es-MX", {
  style: "currency",
  currency: "MXN",
});

const navItems = [
  { label: "Venta", icon: LayoutDashboard, active: true },
  { label: "Productos", icon: Package },
  { label: "Ventas", icon: ReceiptText },
  { label: "Clientes", icon: Users },
];

function App() {
  const [activeCategory, setActiveCategory] = useState("Todos");
  const [query, setQuery] = useState("");
  const [cart, setCart] = useState([
    { product: products[0], quantity: 2 },
    { product: products[4], quantity: 1 },
  ]);
  const [cartOpen, setCartOpen] = useState(false);
  const [checkoutOpen, setCheckoutOpen] = useState(false);
  const [paymentMethod, setPaymentMethod] = useState("Tarjeta");
  const [paymentDone, setPaymentDone] = useState(false);

  const filteredProducts = useMemo(() => {
    const normalizedQuery = query.trim().toLocaleLowerCase("es");

    return products.filter((product) => {
      const categoryMatches =
        activeCategory === "Todos" || product.category === activeCategory;
      const queryMatches =
        !normalizedQuery ||
        product.name.toLocaleLowerCase("es").includes(normalizedQuery) ||
        product.category.toLocaleLowerCase("es").includes(normalizedQuery);

      return categoryMatches && queryMatches;
    });
  }, [activeCategory, query]);

  const itemCount = cart.reduce((total, item) => total + item.quantity, 0);
  const subtotal = cart.reduce(
    (total, item) => total + item.product.price * item.quantity,
    0,
  );
  const tax = subtotal * 0.16;
  const total = subtotal + tax;

  function addProduct(product) {
    setCart((currentCart) => {
      const currentItem = currentCart.find(
        (item) => item.product.id === product.id,
      );

      if (currentItem) {
        return currentCart.map((item) =>
          item.product.id === product.id
            ? {
                ...item,
                quantity: Math.min(item.quantity + 1, product.stock),
              }
            : item,
        );
      }

      return [...currentCart, { product, quantity: 1 }];
    });
  }

  function updateQuantity(productId, nextQuantity) {
    if (nextQuantity <= 0) {
      setCart((currentCart) =>
        currentCart.filter((item) => item.product.id !== productId),
      );
      return;
    }

    setCart((currentCart) =>
      currentCart.map((item) =>
        item.product.id === productId
          ? {
              ...item,
              quantity: Math.min(nextQuantity, item.product.stock),
            }
          : item,
      ),
    );
  }

  function openCheckout() {
    if (!cart.length) return;
    setPaymentDone(false);
    setCheckoutOpen(true);
  }

  function finishPayment() {
    setPaymentDone(true);
  }

  function closeCheckout() {
    if (paymentDone) {
      setCart([]);
      setCartOpen(false);
    }

    setCheckoutOpen(false);
    setPaymentDone(false);
  }

  return (
    <div className="app-shell">
      <aside className="sidebar" aria-label="Navegación principal">
        <div className="brand">
          <span className="brand-mark">
            <Store size={22} strokeWidth={2.3} />
          </span>
          <span className="brand-copy">
            Vende<span>Más</span>
          </span>
        </div>

        <nav className="nav-primary">
          {navItems.map(({ label, icon: Icon, active }) => (
            <button
              className={`nav-item${active ? " active" : ""}`}
              key={label}
              type="button"
              aria-current={active ? "page" : undefined}
            >
              <Icon size={20} />
              <span>{label}</span>
            </button>
          ))}
        </nav>

        <div className="sidebar-bottom">
          <button className="nav-item" type="button">
            <Settings size={20} />
            <span>Ajustes</span>
          </button>
          <div className="profile">
            <span className="avatar">AM</span>
            <span className="profile-copy">
              <strong>Ana Méndez</strong>
              <small>Cajera</small>
            </span>
          </div>
        </div>
      </aside>

      <main className="pos-main">
        <header className="topbar">
          <div>
            <p className="eyebrow">Miércoles, 29 de julio</p>
            <h1>Nueva venta</h1>
          </div>

          <div className="topbar-actions">
            <button className="icon-button notification" type="button">
              <Bell size={20} />
              <span className="notification-dot" />
              <span className="sr-only">Notificaciones</span>
            </button>
            <button className="store-selector" type="button">
              <span className="store-icon">
                <Store size={18} />
              </span>
              <span>
                <small>Sucursal</small>
                <strong>Centro</strong>
              </span>
              <ChevronDown size={17} />
            </button>
          </div>
        </header>

        <section className="catalog-toolbar" aria-label="Buscar productos">
          <label className="search-box">
            <Search size={20} />
            <input
              value={query}
              onChange={(event) => setQuery(event.target.value)}
              placeholder="Buscar por nombre o categoría"
              type="search"
            />
            <span className="shortcut">⌘ K</span>
          </label>
          <button className="scan-button" type="button">
            <Barcode size={21} />
            <span>Escanear</span>
          </button>
        </section>

        <section className="categories" aria-labelledby="categories-title">
          <div className="section-heading">
            <div>
              <p className="eyebrow">Explorar</p>
              <h2 id="categories-title">Categorías</h2>
            </div>
            <span className="product-count">{filteredProducts.length} productos</span>
          </div>

          <div className="category-row">
            {categories.map(({ id, label, icon: Icon }) => (
              <button
                className={`category-chip${activeCategory === id ? " active" : ""}`}
                key={id}
                onClick={() => setActiveCategory(id)}
                type="button"
              >
                <Icon size={18} />
                <span>{label}</span>
              </button>
            ))}
          </div>
        </section>

        <section className="products-section" aria-labelledby="products-title">
          <h2 className="sr-only" id="products-title">
            Productos disponibles
          </h2>
          <div className="product-grid">
            {filteredProducts.map((product) => {
              const Icon = product.icon;
              const inCart = cart.find(
                (item) => item.product.id === product.id,
              );

              return (
                <article className="product-card" key={product.id}>
                  <button
                    className="product-card-action"
                    onClick={() => addProduct(product)}
                    type="button"
                    aria-label={`Agregar ${product.name}`}
                  >
                    <span className={`product-visual ${product.color}`}>
                      <Icon size={46} strokeWidth={1.5} />
                      {inCart && (
                        <span className="in-cart-badge">
                          <Check size={13} strokeWidth={3} />
                          {inCart.quantity}
                        </span>
                      )}
                    </span>
                    <span className="product-info">
                      <span className="product-category">{product.category}</span>
                      <strong>{product.name}</strong>
                      <span className="product-meta">
                        <b>{money.format(product.price)}</b>
                        <small>por {product.unit}</small>
                      </span>
                    </span>
                    <span className="stock">
                      <span
                        className={`stock-dot${product.stock <= 6 ? " low" : ""}`}
                      />
                      {product.stock} disponibles
                    </span>
                    <span className="quick-add">
                      <Plus size={17} />
                      Agregar
                    </span>
                  </button>
                </article>
              );
            })}
          </div>

          {!filteredProducts.length && (
            <div className="empty-search">
              <Search size={26} />
              <h3>No encontramos productos</h3>
              <p>Prueba con otro nombre o selecciona otra categoría.</p>
            </div>
          )}
        </section>
      </main>

      <aside className={`order-panel${cartOpen ? " is-open" : ""}`}>
        <div className="order-header">
          <div>
            <p className="eyebrow">Orden actual</p>
            <h2>Venta #1048</h2>
          </div>
          <button
            className="close-order"
            onClick={() => setCartOpen(false)}
            type="button"
          >
            <X size={20} />
            <span className="sr-only">Cerrar orden</span>
          </button>
          {cart.length > 0 && (
            <button
              className="clear-cart"
              onClick={() => setCart([])}
              type="button"
            >
              Vaciar
            </button>
          )}
        </div>

        <div className="customer-card">
          <span className="customer-icon">
            <Users size={19} />
          </span>
          <span>
            <small>Cliente</small>
            <strong>Público general</strong>
          </span>
          <button type="button">Cambiar</button>
        </div>

        <div className="cart-list">
          {cart.map(({ product, quantity }) => {
            const Icon = product.icon;

            return (
              <article className="cart-item" key={product.id}>
                <span className={`cart-thumb ${product.color}`}>
                  <Icon size={23} strokeWidth={1.6} />
                </span>
                <span className="cart-item-copy">
                  <strong>{product.name}</strong>
                  <small>{money.format(product.price)} c/u</small>
                  <span className="quantity-control">
                    <button
                      onClick={() =>
                        updateQuantity(product.id, quantity - 1)
                      }
                      type="button"
                      aria-label={`Restar ${product.name}`}
                    >
                      <Minus size={14} />
                    </button>
                    <b>{quantity}</b>
                    <button
                      onClick={() =>
                        updateQuantity(product.id, quantity + 1)
                      }
                      type="button"
                      aria-label={`Agregar otra unidad de ${product.name}`}
                    >
                      <Plus size={14} />
                    </button>
                  </span>
                </span>
                <span className="cart-item-total">
                  <strong>{money.format(product.price * quantity)}</strong>
                  <button
                    onClick={() => updateQuantity(product.id, 0)}
                    type="button"
                    aria-label={`Eliminar ${product.name}`}
                  >
                    <Trash2 size={16} />
                  </button>
                </span>
              </article>
            );
          })}

          {!cart.length && (
            <div className="empty-cart">
              <span>
                <ShoppingBasket size={30} />
              </span>
              <h3>Tu orden está vacía</h3>
              <p>Selecciona un producto para comenzar esta venta.</p>
            </div>
          )}
        </div>

        <div className="order-summary">
          <div>
            <span>Subtotal</span>
            <strong>{money.format(subtotal)}</strong>
          </div>
          <div>
            <span>IVA (16%)</span>
            <strong>{money.format(tax)}</strong>
          </div>
          <div className="total-row">
            <span>Total</span>
            <strong>{money.format(total)}</strong>
          </div>
          <button
            className="checkout-button"
            disabled={!cart.length}
            onClick={openCheckout}
            type="button"
          >
            <CreditCard size={21} />
            Cobrar {money.format(total)}
          </button>
          <p className="keyboard-hint">
            Presiona <kbd>F2</kbd> para cobrar
          </p>
        </div>
      </aside>

      <button
        className="mobile-cart-summary"
        onClick={() => setCartOpen(true)}
        type="button"
      >
        <span className="mobile-cart-icon">
          <ShoppingBasket size={20} />
          <b>{itemCount}</b>
        </span>
        <span>
          <small>Ver orden</small>
          <strong>{money.format(total)}</strong>
        </span>
      </button>

      {cartOpen && (
        <button
          aria-label="Cerrar orden"
          className="order-backdrop"
          onClick={() => setCartOpen(false)}
          type="button"
        />
      )}

      {checkoutOpen && (
        <div className="modal-backdrop" role="presentation">
          <section
            aria-labelledby="checkout-title"
            aria-modal="true"
            className="checkout-modal"
            role="dialog"
          >
            {!paymentDone ? (
              <>
                <header className="modal-header">
                  <div>
                    <p className="eyebrow">Finalizar venta</p>
                    <h2 id="checkout-title">Selecciona el método de pago</h2>
                  </div>
                  <button onClick={closeCheckout} type="button">
                    <X size={20} />
                    <span className="sr-only">Cerrar</span>
                  </button>
                </header>

                <div className="checkout-total">
                  <span>Total a cobrar</span>
                  <strong>{money.format(total)}</strong>
                  <small>{itemCount} artículos</small>
                </div>

                <div className="payment-options">
                  {[
                    { label: "Tarjeta", icon: CreditCard },
                    { label: "Efectivo", icon: Banknote },
                    { label: "Transferencia", icon: Landmark },
                  ].map(({ label, icon: Icon }) => (
                    <button
                      className={paymentMethod === label ? "active" : ""}
                      key={label}
                      onClick={() => setPaymentMethod(label)}
                      type="button"
                    >
                      <span>
                        <Icon size={23} />
                      </span>
                      <strong>{label}</strong>
                      {paymentMethod === label && (
                        <CheckCircle2 size={18} className="payment-check" />
                      )}
                    </button>
                  ))}
                </div>

                <button
                  className="confirm-payment"
                  onClick={finishPayment}
                  type="button"
                >
                  Confirmar pago con {paymentMethod.toLowerCase()}
                </button>
              </>
            ) : (
              <div className="payment-success">
                <span>
                  <CheckCircle2 size={44} />
                </span>
                <p className="eyebrow">Pago aprobado</p>
                <h2 id="checkout-title">¡Venta completada!</h2>
                <p>
                  La venta #1048 fue registrada correctamente por{" "}
                  {money.format(total)}.
                </p>
                <button onClick={closeCheckout} type="button">
                  Iniciar nueva venta
                </button>
              </div>
            )}
          </section>
        </div>
      )}
    </div>
  );
}

export default App;
