import { CircleHelp } from "lucide-react";
import { Link } from "react-router-dom";

function NotFoundPage() {
  return (
    <main className="module-main not-found-page">
      <CircleHelp size={42} />
      <p className="eyebrow">Error 404</p>
      <h1>Página no encontrada</h1>
      <p>La dirección solicitada no pertenece a un módulo de VendeMás.</p>
      <Link to="/">Volver a nueva venta</Link>
    </main>
  );
}

export default NotFoundPage;
