import { ArrowLeft, LockKeyhole, Mail, UserRound, UserRoundPlus } from "lucide-react";
import { useState } from "react";
import { Link, Navigate, useNavigate } from "react-router-dom";
import { useAuth } from "../auth/AuthContext.jsx";

const initialForm = {
  firstName: "",
  lastName: "",
  email: "",
  password: "",
  confirmPassword: "",
};

function RegisterPage() {
  const { isAuthenticated, registerCustomer } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState(initialForm);
  const [error, setError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  if (isAuthenticated) {
    return <Navigate replace to="/no-autorizado" />;
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

    if (form.password !== form.confirmPassword) {
      setError("Las contraseñas no coinciden.");
      return;
    }

    setIsSubmitting(true);
    try {
      await registerCustomer({
        firstName: form.firstName.trim(),
        lastName: form.lastName.trim(),
        email: form.email.trim(),
        password: form.password,
      });
      navigate("/no-autorizado", { replace: true });
    } catch (requestError) {
      setError(requestError.message);
    } finally {
      setIsSubmitting(false);
    }
  }

  return (
    <main className="auth-page">
      <section className="auth-card auth-card-wide" aria-labelledby="register-title">
        <Link className="auth-back" to="/login">
          <ArrowLeft size={17} /> Volver
        </Link>

        <div className="auth-heading">
          <p className="eyebrow">Cuenta de cliente</p>
          <h1 id="register-title">Crea tu cuenta</h1>
          <p>Prepárate para comprar en las tiendas de VendeMás.</p>
        </div>

        <form className="auth-form" onSubmit={handleSubmit}>
          <div className="auth-form-row">
            <label>
              Nombre
              <span className="auth-input">
                <UserRound size={18} />
                <input
                  autoComplete="given-name"
                  minLength="2"
                  name="firstName"
                  onChange={handleChange}
                  required
                  value={form.firstName}
                />
              </span>
            </label>
            <label>
              Apellidos
              <span className="auth-input">
                <UserRound size={18} />
                <input
                  autoComplete="family-name"
                  minLength="2"
                  name="lastName"
                  onChange={handleChange}
                  required
                  value={form.lastName}
                />
              </span>
            </label>
          </div>

          <label>
            Correo electrónico
            <span className="auth-input">
              <Mail size={18} />
              <input
                autoComplete="email"
                name="email"
                onChange={handleChange}
                required
                type="email"
                value={form.email}
              />
            </span>
          </label>

          <div className="auth-form-row">
            <label>
              Contraseña
              <span className="auth-input">
                <LockKeyhole size={18} />
                <input
                  autoComplete="new-password"
                  minLength="8"
                  name="password"
                  onChange={handleChange}
                  required
                  type="password"
                  value={form.password}
                />
              </span>
            </label>
            <label>
              Confirma la contraseña
              <span className="auth-input">
                <LockKeyhole size={18} />
                <input
                  autoComplete="new-password"
                  minLength="8"
                  name="confirmPassword"
                  onChange={handleChange}
                  required
                  type="password"
                  value={form.confirmPassword}
                />
              </span>
            </label>
          </div>

          <small className="auth-hint">
            Utiliza al menos 8 caracteres, mayúscula, minúscula, número y símbolo.
          </small>

          {error && <p className="auth-error" role="alert">{error}</p>}

          <button className="auth-submit" disabled={isSubmitting} type="submit">
            <UserRoundPlus size={18} />
            {isSubmitting ? "Creando cuenta…" : "Crear cuenta"}
          </button>
        </form>

        <p className="auth-switch">
          ¿Ya tienes una cuenta? <Link to="/login">Inicia sesión</Link>
        </p>
      </section>
    </main>
  );
}

export default RegisterPage;
