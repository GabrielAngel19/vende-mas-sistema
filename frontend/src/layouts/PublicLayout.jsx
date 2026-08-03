import { Outlet } from "react-router-dom";
import PublicHeader from "../components/storefront/PublicHeader.jsx";

function PublicLayout() {
  return (
    <div className="sf-shell">
      <PublicHeader />
      <Outlet />
    </div>
  );
}

export default PublicLayout;
