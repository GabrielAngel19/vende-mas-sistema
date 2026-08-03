import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "./AuthContext.jsx";

function RoleRoute({ allowedRoles }) {
  const { hasAnyRole } = useAuth();

  if (!hasAnyRole(allowedRoles)) {
    return <Navigate replace to="/no-autorizado" />;
  }

  return <Outlet />;
}

export default RoleRoute;
