import { LogOut, ShieldAlert, Store } from "lucide-react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

function UnauthorizedPage() {
  const { isAuthenticated, user, hasAnyRole, logout } = useAuth();
  const navigate = useNavigate();

  if (!isAuthenticated) {
    return <Navigate replace to="/login" />;
  }

  if (hasAnyRole(["StoreOwner", "StoreEmployee"])) {
    return <Navigate replace to="/app/venta" />;
  }

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  return (
    <main className="auth-page">
      <section className="auth-card auth-message-card">
        <span className="auth-message-icon"><ShieldAlert size={34} /></span>
        <p className="eyebrow">Sesión correcta</p>
        <h1>Hola, {user?.firstName}</h1>
        <p>
          Tu cuenta es de cliente. La tienda virtual estará disponible en el
          siguiente sprint; el punto de venta es exclusivo para tiendas.
        </p>
        <div className="auth-message-actions">
          <button className="auth-submit" onClick={handleLogout} type="button">
            <LogOut size={18} /> Cerrar sesión
          </button>
          <Link className="auth-secondary" to="/">
            <Store size={18} /> Ir al inicio
          </Link>
        </div>
      </section>
    </main>
  );
}

export default UnauthorizedPage;
