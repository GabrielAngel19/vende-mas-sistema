import {
  LayoutDashboard,
  LogOut,
  Package,
  ReceiptText,
  Settings,
  Store,
  Users,
} from "lucide-react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../auth/AuthContext.jsx";

const navItems = [
  { to: "/app/venta", label: "Venta", icon: LayoutDashboard, end: true },
  { to: "/app/productos", label: "Productos", icon: Package },
  { to: "/app/ventas", label: "Ventas", icon: ReceiptText },
  { to: "/app/clientes", label: "Clientes", icon: Users },
];

const roleLabels = {
  StoreOwner: "Propietario",
  StoreEmployee: "Empleado",
};

function navClassName({ isActive }) {
  return `nav-item${isActive ? " active" : ""}`;
}

function getInitials(user) {
  return `${user?.firstName?.[0] ?? ""}${user?.lastName?.[0] ?? ""}`.toUpperCase();
}

function AppSidebar() {
  const { user, activeStore, hasRole, logout } = useAuth();
  const navigate = useNavigate();
  const canManageSettings = hasRole("StoreOwner");
  const primaryRole = user?.roles?.[0];

  function handleLogout() {
    logout();
    navigate("/login", { replace: true });
  }

  return (
    <aside className="sidebar" aria-label="Navegación principal">
      <NavLink className="brand" to="/app/venta" aria-label="Ir a nueva venta">
        <span className="brand-mark">
          <Store size={22} strokeWidth={2.3} />
        </span>
        <span className="brand-copy">
          Vende<span>Más</span>
        </span>
      </NavLink>

      <nav className="nav-primary">
        {navItems.map(({ to, label, icon: Icon, end }) => (
          <NavLink className={navClassName} end={end} key={to} to={to}>
            <Icon size={20} />
            <span>{label}</span>
          </NavLink>
        ))}

        {canManageSettings && (
          <NavLink
            className={({ isActive }) =>
              `${navClassName({ isActive })} mobile-settings-link`
            }
            to="/app/ajustes"
          >
            <Settings size={20} />
            <span>Ajustes</span>
          </NavLink>
        )}
      </nav>

      <div className="sidebar-bottom">
        {canManageSettings && (
          <NavLink className={navClassName} to="/app/ajustes">
            <Settings size={20} />
            <span>Ajustes</span>
          </NavLink>
        )}
        <div className="profile">
          <span className="avatar">{getInitials(user)}</span>
          <span className="profile-copy">
            <strong>{`${user?.firstName ?? ""} ${user?.lastName ?? ""}`}</strong>
            <small>
              {roleLabels[primaryRole] ?? primaryRole}
              {activeStore ? ` · ${activeStore.storeName}` : ""}
            </small>
          </span>
          <button
            aria-label="Cerrar sesión"
            className="logout-button"
            onClick={handleLogout}
            title="Cerrar sesión"
            type="button"
          >
            <LogOut size={18} />
          </button>
        </div>
      </div>
    </aside>
  );
}

export default AppSidebar;
