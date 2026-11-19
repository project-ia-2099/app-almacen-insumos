import { useEffect, useState, type FormEvent } from 'react';
import { createInsumo, deleteInsumo, listInsumos } from './api/insumosApi';
import type { Insumo } from './types/insumo';

export default function App() {
  const [insumos, setInsumos] = useState<Insumo[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [codigo, setCodigo] = useState('');
  const [nombre, setNombre] = useState('');
  const [stock, setStock] = useState('');
  const [unidadMedida, setUnidadMedida] = useState('');

  async function loadInsumos() {
    setLoading(true);
    setError(null);
    try {
      const data = await listInsumos();
      setInsumos(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo cargar el inventario de insumos.');
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    void loadInsumos();
  }, []);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setSubmitting(true);
    setError(null);
    setSuccessMessage(null);

    try {
      const parsedStock = stock === '' ? -1 : Number(stock);
      const created = await createInsumo({
        codigo,
        nombre,
        stock: parsedStock,
        unidadMedida,
      });

      setInsumos((prev) => [created, ...prev]);
      setCodigo('');
      setNombre('');
      setStock('');
      setUnidadMedida('');
      setSuccessMessage(`Insumo "${created.nombre}" registrado correctamente.`);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo registrar el insumo.');
    } finally {
      setSubmitting(false);
    }
  }

  async function handleDelete(id: string) {
    setDeletingId(id);
    setError(null);
    setSuccessMessage(null);

    try {
      await deleteInsumo(id);
      setInsumos((prev) => prev.filter((item) => item.id !== id));
      setSuccessMessage('Insumo eliminado correctamente.');
    } catch (err) {
      setError(err instanceof Error ? err.message : 'No se pudo eliminar el insumo.');
    } finally {
      setDeletingId(null);
    }
  }

  return (
    <main className="page-shell">
      <header className="page-header">
        <div className="header-title-group">
          <div className="header-icon" aria-hidden="true">
            <i className="fa-solid fa-boxes-stacked" />
          </div>
          <div>
            <h1 className="page-title">Gestión de Insumos</h1>
            <p className="page-subtitle">Registro y control de existencias de almacén</p>
          </div>
        </div>
        <span className="badge-area">
          <i className="fa-solid fa-warehouse" aria-hidden="true" />
          Área: Almacén
        </span>
      </header>

      {error && (
        <div className="alert alert-error" role="alert">
          <i className="fa-solid fa-circle-exclamation" aria-hidden="true" />
          <span>{error}</span>
        </div>
      )}

      {successMessage && (
        <div className="alert alert-success" role="status">
          <i className="fa-solid fa-circle-check" aria-hidden="true" />
          <span>{successMessage}</span>
        </div>
      )}

      <section className="card" aria-labelledby="form-heading">
        <h2 id="form-heading" className="card-title">
          <i className="fa-solid fa-plus-circle" aria-hidden="true" />
          Registrar Insumo
        </h2>

        <form className="form-grid" onSubmit={handleSubmit}>
          <div className="field">
            <label htmlFor="codigo">Código</label>
            <input
              id="codigo"
              name="codigo"
              type="text"
              placeholder="Ej. INS-001"
              value={codigo}
              onChange={(e) => setCodigo(e.target.value)}
            />
          </div>

          <div className="field">
            <label htmlFor="nombre">Nombre</label>
            <input
              id="nombre"
              name="nombre"
              type="text"
              placeholder="Ej. Guantes de Nitrilo"
              value={nombre}
              onChange={(e) => setNombre(e.target.value)}
            />
          </div>

          <div className="field">
            <label htmlFor="stock">Stock</label>
            <input
              id="stock"
              name="stock"
              type="number"
              step="0.01"
              placeholder="0"
              value={stock}
              onChange={(e) => setStock(e.target.value)}
            />
          </div>

          <div className="field">
            <label htmlFor="unidadMedida">Unidad de Medida</label>
            <input
              id="unidadMedida"
              name="unidadMedida"
              type="text"
              placeholder="Ej. Cajas, Unidades, Litros"
              value={unidadMedida}
              onChange={(e) => setUnidadMedida(e.target.value)}
            />
          </div>

          <button type="submit" className="btn btn-primary" disabled={submitting}>
            <i className="fa-solid fa-floppy-disk" aria-hidden="true" />
            {submitting ? 'Registrando...' : 'Registrar'}
          </button>
        </form>
      </section>

      <section className="card" aria-labelledby="list-heading">
        <h2 id="list-heading" className="card-title">
          <i className="fa-solid fa-list" aria-hidden="true" />
          Listado de Insumos ({insumos.length})
        </h2>

        {loading ? (
          <div className="empty-state">
            <i className="fa-solid fa-spinner fa-spin" aria-hidden="true" />
            <p>Cargando insumos...</p>
          </div>
        ) : insumos.length === 0 ? (
          <div className="empty-state">
            <i className="fa-solid fa-box-open" aria-hidden="true" />
            <p>No hay insumos registrados en almacén.</p>
          </div>
        ) : (
          <div className="table-wrapper">
            <table className="data-table">
              <thead>
                <tr>
                  <th>Código</th>
                  <th>Nombre</th>
                  <th>Stock</th>
                  <th>Unidad de Medida</th>
                  <th className="text-right">Acciones</th>
                </tr>
              </thead>
              <tbody>
                {insumos.map((item) => (
                  <tr key={item.id}>
                    <td>
                      <span className="code-pill">{item.codigo}</span>
                    </td>
                    <td>{item.nombre}</td>
                    <td>{item.stock}</td>
                    <td>{item.unidadMedida}</td>
                    <td className="text-right">
                      <button
                        type="button"
                        className="btn btn-danger-outline"
                        disabled={deletingId === item.id}
                        onClick={() => void handleDelete(item.id)}
                        aria-label={`Eliminar ${item.nombre}`}
                      >
                        <i className="fa-solid fa-trash-can" aria-hidden="true" />
                        {deletingId === item.id ? 'Eliminando...' : 'Eliminar'}
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </section>
    </main>
  );
}
