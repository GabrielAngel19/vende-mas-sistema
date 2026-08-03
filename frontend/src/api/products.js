import { apiRequest } from "./http.js";

export function getProducts(filters = {}, options = {}) {
  const filterNames = ["search", "category", "storeId", "branchId", "availableOnly"];
  const optionNames = ["signal", "headers", "method", "body"];
  const receivedLegacyOptions =
    optionNames.some((name) => name in filters)
    && !filterNames.some((name) => name in filters);
  const activeFilters = receivedLegacyOptions ? {} : filters;
  const requestOptions = receivedLegacyOptions ? filters : options;
  const params = new URLSearchParams();

  if (activeFilters.search) params.set("search", activeFilters.search);
  if (activeFilters.category) params.set("category", activeFilters.category);
  if (activeFilters.storeId) params.set("storeId", activeFilters.storeId);
  if (activeFilters.branchId) params.set("branchId", activeFilters.branchId);
  if (activeFilters.availableOnly) params.set("availableOnly", "true");

  const query = params.toString();
  return apiRequest(`/api/products${query ? `?${query}` : ""}`, {
    auth: false,
    ...requestOptions,
  });
}

export function getProductById(id, options = {}) {
  return apiRequest(`/api/products/${id}`, {
    auth: false,
    ...options,
  });
}
