import { ArrowRight, CheckCircle2, Construction } from "lucide-react";

function ModulePage({ description, eyebrow, icon: Icon, steps, title }) {
  return (
    <main className="module-main">
      <header className="module-header">
        <div>
          <p className="eyebrow">{eyebrow}</p>
          <h1>{title}</h1>
          <p>{description}</p>
        </div>
        <span className="module-icon" aria-hidden="true">
          <Icon size={30} strokeWidth={1.8} />
        </span>
      </header>

      <section className="module-placeholder" aria-labelledby="module-status">
        <span className="module-status-icon">
          <Construction size={32} />
        </span>
        <div>
          <p className="eyebrow">Próximo incremento</p>
          <h2 id="module-status">Módulo preparado para implementación</h2>
          <p>
            La navegación y la pantalla ya están separadas del punto de venta.
            Las operaciones se agregarán en su propia rama para conservar un
            historial claro y revisable.
          </p>
        </div>
      </section>

      <section className="module-plan" aria-labelledby="module-plan-title">
        <div className="section-heading">
          <div>
            <p className="eyebrow">Criterios de aceptación</p>
            <h2 id="module-plan-title">Siguiente alcance</h2>
          </div>
        </div>

        <div className="module-plan-grid">
          {steps.map((step) => (
            <article key={step}>
              <CheckCircle2 size={20} />
              <span>{step}</span>
              <ArrowRight size={17} />
            </article>
          ))}
        </div>
      </section>
    </main>
  );
}

export default ModulePage;
