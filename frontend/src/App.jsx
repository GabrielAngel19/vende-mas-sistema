import { BrowserRouter, Route, Routes } from "react-router-dom";
import AppLayout from "./layouts/AppLayout.jsx";
import CustomersPage from "./pages/CustomersPage.jsx";
import NotFoundPage from "./pages/NotFoundPage.jsx";
import ProductsPage from "./pages/ProductsPage.jsx";
import SalePage from "./pages/SalePage.jsx";
import SalesPage from "./pages/SalesPage.jsx";
import SettingsPage from "./pages/SettingsPage.jsx";

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route index element={<SalePage />} />
          <Route path="productos" element={<ProductsPage />} />
          <Route path="ventas" element={<SalesPage />} />
          <Route path="clientes" element={<CustomersPage />} />
          <Route path="ajustes" element={<SettingsPage />} />
          <Route path="*" element={<NotFoundPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
