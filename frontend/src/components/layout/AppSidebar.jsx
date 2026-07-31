import {
  LayoutDashboard,
  Package,
  ReceiptText,
  Settings,
  Store,
  Users,
} from "lucide-react";
import { NavLink } from "react-router-dom";

const navItems = [
  { to: "/", label: "Venta", icon: LayoutDashboard, end: true },
  { to: "/productos", label: "Productos", icon: Package },
  { to: "/ventas", label: "Ventas", icon: ReceiptText },
  { to: "/clientes", label: "Clientes", icon: Users },
];

function navClassName({ isActive }) {
  return `nav-item${isActive ? " active" : ""}`;
}

function AppSidebar() {
  return (
    <aside className="sidebar" aria-label="Navegación principal">
      <NavLink className="brand" to="/" aria-label="Ir a nueva venta">
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

        <NavLink
          className={({ isActive }) =>
            `${navClassName({ isActive })} mobile-settings-link`
          }
          to="/ajustes"
        >
          <Settings size={20} />
          <span>Ajustes</span>
        </NavLink>
      </nav>

      <div className="sidebar-bottom">
        <NavLink className={navClassName} to="/ajustes">
          <Settings size={20} />
          <span>Ajustes</span>
        </NavLink>
        <div className="profile">
          <span className="avatar">AM</span>
          <span className="profile-copy">
            <strong>Ana Méndez</strong>
            <small>Cajera</small>
          </span>
        </div>
      </div>
    </aside>
  );
}

export default AppSidebar;
