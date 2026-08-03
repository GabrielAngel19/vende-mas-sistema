import { apiRequest } from "./http.js";

export function getStores(filters = {}, options = {}) {
  const params = new URLSearchParams();
  if (filters.search) params.set("search", filters.search);
  if (filters.city) params.set("city", filters.city);

  const query = params.toString();
  return apiRequest(`/api/stores${query ? `?${query}` : ""}`, {
    auth: false,
    ...options,
  });
}

export function getStoreBySlug(slug, options = {}) {
  return apiRequest(`/api/stores/${encodeURIComponent(slug)}`, {
    auth: false,
    ...options,
  });
}
