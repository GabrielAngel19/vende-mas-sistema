import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import ProtectedRoute from "./auth/ProtectedRoute.jsx";
import RoleRoute from "./auth/RoleRoute.jsx";
import AppLayout from "./layouts/AppLayout.jsx";
import CustomersPage from "./pages/CustomersPage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import NotFoundPage from "./pages/NotFoundPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import SalePage from "./pages/SalePage.jsx";
import SalesPage from "./pages/SalesPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";
import UnauthorizedPage from "./pages/UnauthorizedPage.jsx";

const storeRoles = ["StoreOwner", "StoreEmployee"];

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/registro" element={<RegisterPage />} />
        <Route path="/no-autorizado" element={<UnauthorizedPage />} />
        <Route path="/" element={<Navigate replace to="/app/venta" />} />

        <Route element={<ProtectedRoute />}>
          <Route element={<RoleRoute allowedRoles={storeRoles} />}>
            <Route path="/app" element={<AppLayout />}>
              <Route index element={<Navigate replace to="venta" />} />
              <Route path="venta" element={<SalePage />} />
              <Route path="productos" element={<ProductsPage />} />
              <Route path="ventas" element={<SalesPage />} />
              <Route path="clientes" element={<CustomersPage />} />

              <Route element={<RoleRoute allowedRoles={["StoreOwner"]} />}>
                <Route path="ajustes" element={<SettingsPage />} />
              </Route>

              <Route path="*" element={<NotFoundPage />} />
            </Route>
          </Route>
        </Route>

        <Route path="*" element={<NotFoundPage />} />
      </Routes>
    </BrowserRouter>
  );
}

export default App;
