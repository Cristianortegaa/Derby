export interface Usuario {
  id: number;
  email: string;
  rol: string;
  token: string;
  arbitroId?: number;
  nombreArbitro?: string;
}

export interface LoginRequest {
  email: string;
  contrasena: string;
}

export interface RegistroRequest {
  email: string;
  contrasena: string;
  rol?: string;
}
