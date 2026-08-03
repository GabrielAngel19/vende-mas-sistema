import { createContext, useContext, useMemo, useState } from "react";

const STORAGE_KEY = "vendemas.cart.v1";
const CartContext = createContext(null);

function readCart() {
  try {
    const value = JSON.parse(localStorage.getItem(STORAGE_KEY) ?? "[]");
    return Array.isArray(value) ? value : [];
  } catch {
    return [];
  }
}

export function CartProvider({ children }) {
  const [items, setItems] = useState(readCart);

  function updateCart(updater) {
    setItems((current) => {
      const next = updater(current);
      localStorage.setItem(STORAGE_KEY, JSON.stringify(next));
      return next;
    });
  }

  function addItem(product) {
    updateCart((current) => {
      const existing = current.find((item) => item.id === product.id);
      const maximum = Math.max(0, product.stock ?? 0);
      if (existing) {
        return current.map((item) =>
          item.id === product.id
            ? { ...item, quantity: Math.min(item.quantity + 1, maximum) }
            : item,
        );
      }

      return [
        ...current,
        {
          id: product.id,
          name: product.name,
          price: product.price,
          stock: product.stock,
          unit: product.unit,
          imageUrl: product.imageUrl,
          storeId: product.storeId,
          storeName: product.storeName,
          quantity: 1,
        },
      ];
    });
  }

  function setQuantity(id, quantity) {
    updateCart((current) =>
      current
        .map((item) =>
          item.id === id
            ? { ...item, quantity: Math.max(0, Math.min(quantity, item.stock)) }
            : item,
        )
        .filter((item) => item.quantity > 0),
    );
  }

  function removeItem(id) {
    updateCart((current) => current.filter((item) => item.id !== id));
  }

  const value = useMemo(() => ({
    items,
    itemCount: items.reduce((total, item) => total + item.quantity, 0),
    subtotal: items.reduce(
      (total, item) => total + Number(item.price) * item.quantity,
      0,
    ),
    addItem,
    setQuantity,
    removeItem,
    clear: () => updateCart(() => []),
  }), [items]);

  return <CartContext.Provider value={value}>{children}</CartContext.Provider>;
}

export function useCart() {
  const value = useContext(CartContext);
  if (!value) throw new Error("useCart debe utilizarse dentro de CartProvider.");
  return value;
}
