const SESSION_KEY = "vendemas.auth";

export function readAuthSession() {
  try {
    const rawSession = sessionStorage.getItem(SESSION_KEY);
    if (!rawSession) {
      return null;
    }

    const session = JSON.parse(rawSession);
    const expiresAt = Date.parse(session.expiresAtUtc);

    if (!session.accessToken || !Number.isFinite(expiresAt) || expiresAt <= Date.now()) {
      clearAuthSession();
      return null;
    }

    return session;
  } catch {
    clearAuthSession();
    return null;
  }
}

export function writeAuthSession(session) {
  sessionStorage.setItem(SESSION_KEY, JSON.stringify(session));
  return session;
}

export function clearAuthSession() {
  sessionStorage.removeItem(SESSION_KEY);
}

export function getAccessToken() {
  return readAuthSession()?.accessToken ?? null;
}
