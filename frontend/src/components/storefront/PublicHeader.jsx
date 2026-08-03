import { LogOut, MapPin, Menu, ShoppingBag, Store, UserRound } from "lucide-react";
import { Link, NavLink } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext.jsx";
import { useCart } from "../../storefront/CartContext.jsx";

function PublicHeader() {
  const { user, isAuthenticated, hasAnyRole, logout } = useAuth();
  const { itemCount } = useCart();
  const isStoreUser = hasAnyRole(["StoreOwner", "StoreEmployee"]);

  return (
    <header className="sf-header">
      <div className="sf-header-inner">
        <Link className="sf-brand" to="/">
          <span><Store size={22} /></span>
          <strong>Vende<i>Más</i></strong>
        </Link>

        <nav className="sf-nav" aria-label="Navegación principal">
          <NavLink to="/">Explorar</NavLink>
          <NavLink to="/tiendas"><MapPin size={16} /> Tiendas</NavLink>
          <NavLink to="/quiero-vender">Quiero vender</NavLink>
        </nav>

        <div className="sf-header-actions">
          <Link className="sf-cart-link" to="/carrito" aria-label={`Carrito con ${itemCount} productos`}>
            <ShoppingBag size={19} />
            <span>Carrito</span>
            {itemCount > 0 && <b>{itemCount}</b>}
          </Link>

          {isAuthenticated ? (
            <div className="sf-session">
              <div>
                <small>Hola</small>
                <strong>{user?.firstName}</strong>
              </div>
              {isStoreUser && <Link to="/app/venta">Panel</Link>}
              {!isStoreUser && <Link to="/mis-pedidos">Pedidos</Link>}
              <button onClick={logout} title="Cerrar sesión" type="button">
                <LogOut size={17} />
              </button>
            </div>
          ) : (
            <Link className="sf-login" to="/login">
              <UserRound size={18} /> Ingresar
            </Link>
          )}
          <button className="sf-mobile-menu" type="button" aria-label="Abrir menú">
            <Menu size={20} />
          </button>
        </div>
      </div>
    </header>
  );
}

export default PublicHeader;
