import {Injectable} from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = 'https://derby-production.up.railway.app/api';

  constructor(private http: HttpClient) {
  }

  // Usuarios
  obtenerUsuarios(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/usuarios`);
  }

  obtenerUsuario(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/usuarios/${id}`);
  }

  crearUsuario(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/usuarios/registro`, datos);
  }

  actualizarUsuario(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/usuarios/${id}`, datos);
  }

  eliminarUsuario(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/usuarios/${id}`);
  }

  // Competiciones
  obtenerCompeticiones(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/competiciones`);
  }

  crearCompeticion(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/competiciones`, datos);
  }

  actualizarCompeticion(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/competiciones/${id}`, datos);
  }

  eliminarCompeticion(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/competiciones/${id}`);
  }

  // Equipos
  obtenerEquipos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/equipos`);
  }

  obtenerEquiposSinLiga(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/equipos/sin-liga`);
  }

  crearEquipo(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/equipos`, datos);
  }

  actualizarEquipo(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/equipos/${id}`, datos);
  }

  eliminarEquipo(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/equipos/${id}`);
  }

  // Árbitros
  obtenerArbitros(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/arbitros`);
  }

  crearArbitro(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/arbitros`, datos);
  }

  actualizarArbitro(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/arbitros/${id}`, datos);
  }

  eliminarArbitro(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/arbitros/${id}`);
  }

  // Actas
  obtenerActas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/actas`);
  }

  obtenerEventosPartido(partidoId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitro/partidos/${partidoId}/eventos`);
  }

  crearActa(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/actas`, datos);
  }

  actualizarActa(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/actas/${id}`, datos);
  }

  eliminarActa(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/actas/${id}`);
  }

  obtenerLigas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/ligas`);
  }

  crearLiga(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/ligas`, datos);
  }

  actualizarLiga(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/ligas/${id}`, datos);
  }

  eliminarLiga(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/ligas/${id}`);
  }

  // Partidos
  obtenerPartidos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/partidos`);
  }

  crearPartido(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/partidos`, datos);
  }

  actualizarPartido(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/partidos/${id}`, datos);
  }

  eliminarPartido(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/partidos/${id}`);
  }

  obtenerEquiposLiga(ligaId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/ligas/${ligaId}/equipos`);
  }

  añadirEquipoLiga(ligaId: number, equipoId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/ligas/${ligaId}/equipos`, equipoId);
  }

  quitarEquipoLiga(ligaId: number, equipoId: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/ligas/${ligaId}/equipos/${equipoId}`);
  }

  generarCalendario(ligaId: number): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/ligas/${ligaId}/generar-calendario`, {});
  }

  obtenerJugadores(equipoId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/equipos/${equipoId}/jugadores`);
  }

  agregarJugador(equipoId: number, jugador: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/equipos/${equipoId}/jugadores`, jugador);
  }

  actualizarJugador(id: number, jugador: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/jugadores/${id}`, jugador);
  }

  eliminarJugador(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/jugadores/${id}`);
  }
}
