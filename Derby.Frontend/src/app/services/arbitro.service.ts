import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ArbitroService {
  private apiUrl = 'http://localhost:5101/api';

  constructor(private http: HttpClient) {}

  // Mis Partidos
  obtenerMisPartidos(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitros/${arbitroId}/partidos`);
  }

  obtenerPartidosPendientes(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitros/${arbitroId}/partidos/pendientes`);
  }

  // Actas
  obtenerActasDelArbitro(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitros/${arbitroId}/actas`);
  }

  obtenerActa(partideId: number): Observable<any> {
    return this.http.get(`${this.apiUrl}/partidos/${partideId}/acta`);
  }

  crearActa(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/actas`, datos);
  }

  actualizarActa(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/actas/${id}`, datos);
  }

  enviarActa(id: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/actas/${id}/enviar`, {});
  }

  // Historial
  obtenerHistorialPartidos(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitros/${arbitroId}/historial`);
  }
}

