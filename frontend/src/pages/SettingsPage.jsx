import { Settings } from "lucide-react";
import ModulePage from "../components/layout/ModulePage.jsx";

function SettingsPage() {
  return (
    <ModulePage
      description="Centraliza la configuración de sucursales y preferencias operativas."
      eyebrow="Configuración"
      icon={Settings}
      title="Ajustes"
      steps={[
        "Administrar sucursales",
        "Definir preferencias de la venta",
        "Configurar datos del negocio",
      ]}
    />
  );
}

export default SettingsPage;
