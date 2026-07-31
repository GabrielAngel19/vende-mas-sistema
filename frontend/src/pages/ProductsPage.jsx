import { PackageSearch } from "lucide-react";
import ModulePage from "../components/layout/ModulePage.jsx";

function ProductsPage() {
  return (
    <ModulePage
      description="Administra el catálogo y las existencias disponibles para el punto de venta."
      eyebrow="Catálogo"
      icon={PackageSearch}
      title="Productos"
      steps={[
        "Consultar y buscar productos",
        "Crear y editar información",
        "Activar o desactivar productos",
      ]}
    />
  );
}

export default ProductsPage;
