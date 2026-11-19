import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi } from 'vitest';
import App from './App';
import * as insumosApi from './api/insumosApi';

vi.mock('./api/insumosApi');

describe('App', () => {
  beforeEach(() => {
    vi.resetAllMocks();
  });

  it('renders listed insumos and allows registering and deleting an insumo', async () => {
    const user = userEvent.setup();
    vi.mocked(insumosApi.listInsumos).mockResolvedValueOnce([
      {
        id: '1',
        codigo: 'INS-001',
        nombre: 'Guantes de Nitrilo',
        stock: 100,
        unidadMedida: 'Cajas',
        createdAtUtc: '2026-01-01T00:00:00Z',
      },
    ]);

    vi.mocked(insumosApi.createInsumo).mockResolvedValueOnce({
      id: '2',
      codigo: 'INS-002',
      nombre: 'Mascarilla KN95',
      stock: 50,
      unidadMedida: 'Cajas',
      createdAtUtc: '2026-01-02T00:00:00Z',
    });

    vi.mocked(insumosApi.deleteInsumo).mockResolvedValueOnce();

    render(<App />);

    expect(await screen.findByText('Guantes de Nitrilo')).toBeInTheDocument();

    await user.type(screen.getByLabelText('Código'), 'INS-002');
    await user.type(screen.getByLabelText('Nombre'), 'Mascarilla KN95');
    await user.type(screen.getByLabelText('Stock'), '50');
    await user.type(screen.getByLabelText('Unidad de Medida'), 'Cajas');
    await user.click(screen.getByRole('button', { name: /registrar/i }));

    expect(await screen.findByText('Mascarilla KN95')).toBeInTheDocument();

    await user.click(screen.getByRole('button', { name: /eliminar guantes de nitrilo/i }));

    await waitFor(() => {
      expect(screen.queryByText('Guantes de Nitrilo')).not.toBeInTheDocument();
    });
  });

  it('displays backend error message when loading fails', async () => {
    vi.mocked(insumosApi.listInsumos).mockRejectedValueOnce(
      new Error('Error de conexión con la API')
    );

    render(<App />);

    expect(await screen.findByRole('alert')).toHaveTextContent('Error de conexión con la API');
  });
});
