import { Outlet, useLocation } from "react-router-dom";
import AppSidebar from "../components/layout/AppSidebar.jsx";

function AppLayout() {
  const location = useLocation();
  const isSaleRoute = location.pathname === "/";

  return (
    <div className={`app-shell ${isSaleRoute ? "sale-route" : "module-route"}`}>
      <AppSidebar />
      <Outlet />
    </div>
  );
}

export default AppLayout;
