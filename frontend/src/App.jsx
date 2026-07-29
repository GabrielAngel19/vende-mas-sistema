import { useState } from "react";

function App() {
  const [mensaje, setMensaje] = useState("");

  async function obtenerSaludo() {
    const respuesta = await fetch("/api/saludo");
    const datos = await respuesta.json();

    setMensaje(datos.mensaje);
  }

  return (
    <main>
      <h1>React + C#</h1>

      <button onClick={obtenerSaludo}>
        Consultar backend
      </button>

      <p>{mensaje}</p>
    </main>
  );
}

export default App;