import { apiRequest } from "./http.js";

export function getCatalogProducts(filters = {}, options = {}) {
  const params = new URLSearchParams();

  if (filters.search) params.set("search", filters.search);
  if (filters.category) params.set("category", filters.category);
  if (filters.storeId) params.set("storeId", filters.storeId);
  if (filters.city) params.set("city", filters.city);
  if (filters.availableOnly !== false) params.set("availableOnly", "true");

  const query = params.toString();
  return apiRequest(`/api/catalog/products${query ? `?${query}` : ""}`, {
    auth: false,
    ...options,
  });
}

export function getCatalogProductById(id, options = {}) {
  return apiRequest(`/api/catalog/products/${id}`, {
    auth: false,
    ...options,
  });
}
