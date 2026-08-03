import { Navigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

function UnauthorizedPage() {
  const { isAuthenticated, hasAnyRole } = useAuth();

  if (!isAuthenticated) {
    return <Navigate replace to="/login" />;
  }

  if (hasAnyRole(["StoreOwner", "StoreEmployee"])) {
    return <Navigate replace to="/app/venta" />;
  }

  return <Navigate replace to="/" />;
}

export default UnauthorizedPage;
