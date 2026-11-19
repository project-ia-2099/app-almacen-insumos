export interface Insumo {
  id: string;
  codigo: string;
  nombre: string;
  stock: number;
  unidadMedida: string;
  createdAtUtc: string;
}

export interface CreateInsumoPayload {
  codigo: string;
  nombre: string;
  stock: number;
  unidadMedida: string;
}
