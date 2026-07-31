import { Users } from "lucide-react";
import ModulePage from "../components/layout/ModulePage.jsx";

function CustomersPage() {
  return (
    <ModulePage
      description="Gestiona los clientes que pueden asociarse a una venta del negocio."
      eyebrow="Relaciones"
      icon={Users}
      title="Clientes"
      steps={[
        "Consultar y buscar clientes",
        "Crear y editar perfiles",
        "Seleccionar un cliente durante la venta",
      ]}
    />
  );
}

export default CustomersPage;
