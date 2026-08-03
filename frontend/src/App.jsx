import { BrowserRouter, Navigate, Route, Routes } from "react-router-dom";
import ProtectedRoute from "./auth/ProtectedRoute.jsx";
import RoleRoute from "./auth/RoleRoute.jsx";
import AppLayout from "./layouts/AppLayout.jsx";
import PublicLayout from "./layouts/PublicLayout.jsx";
import BecomeSellerPage from "./pages/BecomeSellerPage.jsx";
import CartPage from "./pages/CartPage.jsx";
import CustomersPage from "./pages/CustomersPage.jsx";
import LoginPage from "./pages/LoginPage.jsx";
import MarketplacePage from "./pages/MarketplacePage.jsx";
import NotFoundPage from "./pages/NotFoundPage.jsx";
import OrdersPage from "./pages/OrdersPage.jsx";
import ProductDetailPage from "./pages/ProductDetailPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import RegisterPage from "./pages/RegisterPage.jsx";
import SalePage from "./pages/SalePage.jsx";
import SalesPage from "./pages/SalesPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";
import StoreDetailPage from "./pages/StoreDetailPage.jsx";
import StoresPage from "./pages/StoresPage.jsx";
import UnauthorizedPage from "./pages/UnauthorizedPage.jsx";

const storeRoles = ["StoreOwner", "StoreEmployee"];

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/login" element={<LoginPage />} />
        <Route path="/registro" element={<RegisterPage />} />
        <Route path="/no-autorizado" element={<UnauthorizedPage />} />

        <Route path="/" element={<PublicLayout />}>
          <Route index element={<MarketplacePage />} />
          <Route path="tiendas" element={<StoresPage />} />
          <Route path="tiendas/:slug" element={<StoreDetailPage />} />
          <Route path="productos/:id" element={<ProductDetailPage />} />
          <Route path="carrito" element={<CartPage />} />
          <Route path="quiero-vender" element={<BecomeSellerPage />} />

          <Route element={<ProtectedRoute />}>
            <Route path="mis-pedidos" element={<OrdersPage />} />
          </Route>
        </Route>

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
