async function getErrorMessage(response) {
  try {
    const problem = await response.json();
    return (
      problem.detail ||
      problem.title ||
      problem.message ||
      `La solicitud falló con código ${response.status}.`
    );
  } catch {
    return `La solicitud falló con código ${response.status}.`;
  }
}

export async function apiRequest(path, options = {}) {
  const response = await fetch(path, {
    ...options,
    headers: {
      ...(options.body ? { "Content-Type": "application/json" } : {}),
      ...options.headers,
    },
  });

  if (!response.ok) {
    throw new Error(await getErrorMessage(response));
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}
