import { createContext, useContext, useEffect, useMemo, useState } from "react";
import {
  getCurrentUser,
  login as loginRequest,
  registerCustomer as registerCustomerRequest,
} from "../api/auth.js";
import {
  clearAuthSession,
  readAuthSession,
  writeAuthSession,
} from "./session.js";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [session, setSession] = useState(() => readAuthSession());
  const [isChecking, setIsChecking] = useState(() => Boolean(readAuthSession()));

  useEffect(() => {
    let active = true;
    const storedSession = readAuthSession();

    async function validateSession() {
      if (!storedSession) {
        setIsChecking(false);
        return;
      }

      try {
        const refreshedSession = await getCurrentUser();
        if (active) {
          writeAuthSession(refreshedSession);
          setSession(refreshedSession);
        }
      } catch {
        if (active) {
          clearAuthSession();
          setSession(null);
        }
      } finally {
        if (active) {
          setIsChecking(false);
        }
      }
    }

    validateSession();

    function handleUnauthorized() {
      clearAuthSession();
      setSession(null);
      setIsChecking(false);
    }

    window.addEventListener("auth:unauthorized", handleUnauthorized);
    return () => {
      active = false;
      window.removeEventListener("auth:unauthorized", handleUnauthorized);
    };
  }, []);

  async function authenticate(request, payload) {
    const authSession = await request(payload);
    writeAuthSession(authSession);
    setSession(authSession);
    return authSession;
  }

  function logout() {
    clearAuthSession();
    setSession(null);
  }

  const value = useMemo(() => {
    const roles = session?.user?.roles ?? [];
    const stores = session?.user?.stores ?? [];

    return {
      session,
      user: session?.user ?? null,
      stores,
      activeStore: stores[0] ?? null,
      isAuthenticated: Boolean(session?.accessToken),
      isChecking,
      login: (credentials) => authenticate(loginRequest, credentials),
      registerCustomer: (customer) =>
        authenticate(registerCustomerRequest, customer),
      logout,
      hasRole: (role) => roles.includes(role),
      hasAnyRole: (allowedRoles) =>
        allowedRoles.some((role) => roles.includes(role)),
    };
  }, [session, isChecking]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error("useAuth debe utilizarse dentro de AuthProvider.");
  }

  return context;
}
