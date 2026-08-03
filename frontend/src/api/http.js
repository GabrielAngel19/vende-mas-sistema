import {
  clearAuthSession,
  getAccessToken,
} from "../auth/session.js";

const API_URL = (import.meta.env.VITE_API_URL || "").replace(/\/$/, "");

export class ApiError extends Error {
  constructor(message, status, problem = null) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.problem = problem;
  }
}

async function readProblem(response) {
  try {
    return await response.json();
  } catch {
    return null;
  }
}

function getErrorMessage(problem, status) {
  if (problem?.detail || problem?.title || problem?.message) {
    return problem.detail || problem.title || problem.message;
  }

  if (problem?.errors) {
    const validationMessage = Object.values(problem.errors).flat().join(" ");
    if (validationMessage) {
      return validationMessage;
    }
  }

  return `La solicitud falló con código ${status}.`;
}

export async function apiRequest(path, options = {}) {
  const {
    auth = true,
    headers: customHeaders = {},
    ...fetchOptions
  } = options;
  const token = auth ? getAccessToken() : null;
  const hasJsonBody = fetchOptions.body && !(fetchOptions.body instanceof FormData);

  const response = await fetch(`${API_URL}${path}`, {
    ...fetchOptions,
    headers: {
      ...(hasJsonBody ? { "Content-Type": "application/json" } : {}),
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...customHeaders,
    },
  });

  if (!response.ok) {
    const problem = await readProblem(response);

    if (response.status === 401 && token) {
      clearAuthSession();
      window.dispatchEvent(new Event("auth:unauthorized"));
    }

    throw new ApiError(
      getErrorMessage(problem, response.status),
      response.status,
      problem,
    );
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}
