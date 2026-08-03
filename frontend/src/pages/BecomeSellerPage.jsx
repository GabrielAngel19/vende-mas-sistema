import { ArrowRight, BadgeCheck, Boxes, MapPin, Store } from "lucide-react";
import { Link } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

function BecomeSellerPage() {
  const { isAuthenticated, hasAnyRole } = useAuth();
  const storeUser = hasAnyRole(["StoreOwner", "StoreEmployee"]);

  return (
    <main className="sf-container sf-page">
      <section className="sf-seller-hero">
        <div>
          <p className="sf-eyebrow">Vende en tu comunidad</p>
          <h1>Haz visible tu tienda y administra tus ventas.</h1>
          <p>Registra tu negocio, sus sucursales y productos para que los clientes sepan exactamente dónde encontrarlos.</p>
          {storeUser ? (
            <Link className="sf-primary-button" to="/app/venta">Ir al panel de mi tienda <ArrowRight size={17} /></Link>
          ) : isAuthenticated ? (
            <p className="sf-notice">Tu cuenta ya puede comprar. La conversión guiada de cliente a vendedor será el siguiente módulo; no necesitas crear otra cuenta.</p>
          ) : (
            <div className="sf-seller-actions"><Link className="sf-primary-button" to="/registro">Crear cuenta</Link><Link className="sf-outline-button" to="/login">Ya tengo cuenta</Link></div>
          )}
        </div>
        <div className="sf-seller-card"><Store size={54} /><strong>Tu tienda en VendeMás</strong><span>Catálogo + ubicación + punto de venta</span></div>
      </section>
      <section className="sf-benefits">
        <article><MapPin /><h2>Ubicación clara</h2><p>Publica dirección, ciudad y posición de cada sucursal.</p></article>
        <article><Boxes /><h2>Inventario real</h2><p>Relaciona existencias con productos y sucursales.</p></article>
        <article><BadgeCheck /><h2>Cuenta con roles</h2><p>Dueño y empleados administran únicamente su tienda.</p></article>
      </section>
    </main>
  );
}

export default BecomeSellerPage;
