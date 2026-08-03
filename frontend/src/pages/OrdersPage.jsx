import { ClipboardList } from "lucide-react";
import { Link } from "react-router-dom";

function OrdersPage() {
  return (
    <main className="sf-container sf-page">
      <div className="sf-page-heading"><p className="sf-eyebrow">Mi cuenta</p><h1>Mis pedidos</h1></div>
      <div className="sf-empty">
        <ClipboardList size={42} />
        <h2>Aún no tienes pedidos</h2>
        <p>El historial aparecerá aquí cuando habilitemos el checkout en línea.</p>
        <Link className="sf-primary-button" to="/">Explorar productos</Link>
      </div>
    </main>
  );
}

export default OrdersPage;
