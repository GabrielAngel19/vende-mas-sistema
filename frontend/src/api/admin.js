import { apiRequest } from "./http.js";

function createQuery(values) {
  const query = new URLSearchParams();

  Object.entries(values).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== "") {
      query.set(key, String(value));
    }
  });

  return query.toString();
}

export function getAdminProducts(storeId, { search = "", signal } = {}) {
  const query = createQuery({
    storeId,
    search: search.trim(),
    includeInactive: true,
  });

  return apiRequest(`/api/products?${query}`, { signal });
}

export function createProduct(payload) {
  return apiRequest("/api/products", {
    method: "POST",
    body: JSON.stringify(payload),
  });
}

export function updateProduct(productId, payload) {
  return apiRequest(`/api/products/${productId}`, {
    method: "PUT",
    body: JSON.stringify(payload),
  });
}

export function deactivateProduct(productId) {
  return apiRequest(`/api/products/${productId}`, {
    method: "DELETE",
  });
}
