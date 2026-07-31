import { ReceiptText } from "lucide-react";
import ModulePage from "../components/layout/ModulePage.jsx";

function SalesPage() {
  return (
    <ModulePage
      description="Consulta las ventas registradas y revisa sus partidas y métodos de pago."
      eyebrow="Seguimiento"
      icon={ReceiptText}
      title="Ventas"
      steps={[
        "Listar ventas recientes",
        "Filtrar por fecha y método de pago",
        "Consultar el detalle de cada venta",
      ]}
    />
  );
}

export default SalesPage;
