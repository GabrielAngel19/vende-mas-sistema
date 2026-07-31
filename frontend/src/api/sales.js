import { apiRequest } from "./http.js";

export function createSale(sale) {
  return apiRequest("/api/sales", {
    method: "POST",
    body: JSON.stringify(sale),
  });
}
