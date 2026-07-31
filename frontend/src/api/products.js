import { apiRequest } from "./http.js";

export function getProducts(options = {}) {
  return apiRequest("/api/products", options);
}
