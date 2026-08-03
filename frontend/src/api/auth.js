import { apiRequest } from "./http.js";

export function login(credentials) {
  return apiRequest("/api/auth/login", {
    method: "POST",
    body: JSON.stringify(credentials),
    auth: false,
  });
}

export function registerCustomer(customer) {
  return apiRequest("/api/auth/register/customer", {
    method: "POST",
    body: JSON.stringify(customer),
    auth: false,
  });
}

export function getCurrentUser() {
  return apiRequest("/api/auth/me");
}
