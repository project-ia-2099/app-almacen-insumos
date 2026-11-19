import type { CreateInsumoPayload, Insumo } from '../types/insumo';

function getApiBaseUrl(): string {
  const apiUrl = import.meta.env.VITE_API_URL;
  if (!apiUrl || apiUrl.trim() === '') {
    throw new Error('VITE_API_URL no está configurado en el entorno.');
  }
  return apiUrl.replace(/\/+$/, '');
}

async function parseErrorMessage(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { message?: string };
    if (body.message) {
      return body.message;
    }
  } catch {
    // Response is not JSON; fall through to HTTP status error.
  }
  return `Error del servidor (HTTP ${response.status}).`;
}

export async function listInsumos(): Promise<Insumo[]> {
  const response = await fetch(`${getApiBaseUrl()}/api/insumos`);
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
  return (await response.json()) as Insumo[];
}

export async function createInsumo(payload: CreateInsumoPayload): Promise<Insumo> {
  const response = await fetch(`${getApiBaseUrl()}/api/insumos`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
  return (await response.json()) as Insumo;
}

export async function deleteInsumo(id: string): Promise<void> {
  const response = await fetch(`${getApiBaseUrl()}/api/insumos/${id}`, {
    method: 'DELETE',
  });
  if (!response.ok) {
    throw new Error(await parseErrorMessage(response));
  }
}
