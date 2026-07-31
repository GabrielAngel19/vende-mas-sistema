import { useCallback, useEffect, useState } from "react";
import { getProducts } from "../api/products.js";

export function useProducts() {
  const [products, setProducts] = useState([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState("");

  const loadProducts = useCallback(async (options = {}) => {
    setIsLoading(true);
    setError("");

    try {
      setProducts(await getProducts(options));
    } catch (requestError) {
      if (!options.signal?.aborted) {
        setError(
          requestError instanceof Error
            ? requestError.message
            : "No fue posible cargar los productos.",
        );
      }
    } finally {
      if (!options.signal?.aborted) {
        setIsLoading(false);
      }
    }
  }, []);

  useEffect(() => {
    const controller = new AbortController();
    loadProducts({ signal: controller.signal });

    return () => controller.abort();
  }, [loadProducts]);

  return {
    products,
    isLoading,
    error,
    reload: loadProducts,
  };
}
