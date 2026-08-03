import { LockKeyhole, LogIn, Mail, Store } from "lucide-react";
import { useState } from "react";
import { Link, Navigate, useLocation, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

function LoginPage() {
  const { isAuthenticated, hasAnyRole, login } = useAuth();
  const location = useLocation();
  const navigate = useNavigate();
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (isAuthenticated) {
    return (
      <Navigate
        replace
        to={hasAnyRole(["StoreOwner", "StoreEmployee"]) ? "/app/venta" : "/no-autorizado"}
      />
    );
  }

  function handleChange(event) {
    setForm((current) => ({
      ...current,
      [event.target.name]: event.target.value,
    }));
  }

  async function handleSubmit(event) {
    event.preventDefault();
    setError("");
    setIsSubmitting(true);

    try {
      const authSession = await login({
        email: form.email.trim(),
        password: form.password,
      });
      const storeUser = authSession.user.roles.some((role) =>
        ["StoreOwner", "StoreEmployee"].includes(role),
      );
      const requestedPath = location.state?.from?.pathname;

      navigate(
        storeUser ? requestedPath || "/app/venta" : "/no-autorizado",
        { replace: true },
      );
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card" aria-labelledby="login-title">
        <div className="auth-brand">
          <span><Store size={25} /></span>
          <strong>Vende<i>Más</i></strong>
        </div>

        <div className="auth-heading">
          <p className="eyebrow">Administración de tienda</p>
          <h1 id="login-title">Inicia sesión</h1>
          <p>Accede al punto de venta y administra tu tienda.</p>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
          <label>
            Correo electrónico
            <span className="auth-input">
              <Mail size={18} />
              <input
                autoComplete="email"
                name="email"
                onChange={handleChange}
                placeholder="tu@tienda.com"
                required
                type="email"
                value={form.email}
              />
            </span>
          </label>

          <label>
            Contraseña
            <span className="auth-input">
              <LockKeyhole size={18} />
              <input
                autoComplete="current-password"
                minLength="8"
                name="password"
                onChange={handleChange}
                required
                type="password"
                value={form.password}
              />
            </span>
          </label>

          {error && <p className="auth-error" role="alert">{error}</p>}

          <button className="auth-submit" disabled={isSubmitting} type="submit">
            <LogIn size={18} />
            {isSubmitting ? "Ingresando…" : "Ingresar"}
          </button>
        </form>

        <p className="auth-switch">
          ¿Comprarás en VendeMás? <Link to="/registro">Crea tu cuenta</Link>
        </p>
      </section>
    </main>
  );
}

export default LoginPage;
