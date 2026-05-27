import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ArbitroService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  obtenerMisPartidos(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitro/${arbitroId}/partidos`);
  }

  obtenerPartidosPendientes(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitro/${arbitroId}/partidos/pendientes`);
  }


  obtenerEventos(partidoId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitro/partidos/${partidoId}/eventos`);
  }

  añadirEvento(partidoId: number, dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/arbitro/partidos/${partidoId}/eventos`, dto);
  }

  eliminarEvento(partidoId: number, eventoId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/arbitro/partidos/${partidoId}/eventos/${eventoId}`);
  }

  cerrarActa(partidoId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/arbitro/partidos/${partidoId}/cerrar`, {});
  }

  obtenerHistorialPartidos(arbitroId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitro/${arbitroId}/historial`);
  }
}

