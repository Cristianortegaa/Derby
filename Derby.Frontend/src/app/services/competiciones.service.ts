import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  JornadaResponseDto,
  PartidoResponseDto,
  EquipoResponseDto,
  ResultadoPartidoResponseDto,
  EquipoClasificacionResponseDto,
  GoleadorResponseDto
} from '../models/competicion.model';


@Injectable({
  providedIn: 'root'
})
export class CompeticionesService {
  private apiUrl = 'https://derby-production.up.railway.app/api/competiciones';

  constructor(private http: HttpClient) {}

  obtenerJornadas(competicionId: number): Observable<JornadaResponseDto[]> {
    return this.http.get<JornadaResponseDto[]>(`${this.apiUrl}/${competicionId}/jornadas`);
  }

  obtenerResultados(competicionId: number): Observable<ResultadoPartidoResponseDto[]> {
    return this.http.get<ResultadoPartidoResponseDto[]>(`${this.apiUrl}/${competicionId}/resultados`);
  }

  obtenerClasificacion(competicionId: number): Observable<EquipoClasificacionResponseDto[]> {
    return this.http.get<EquipoClasificacionResponseDto[]>(`${this.apiUrl}/${competicionId}/clasificacion`);
  }

  obtenerGoleadores(competicionId: number): Observable<GoleadorResponseDto[]> {
    return this.http.get<GoleadorResponseDto[]>(`${this.apiUrl}/${competicionId}/goleadores`);
  }

  obtenerJornadasPorLiga(ligaId: number): Observable<JornadaResponseDto[]> {
    return this.http.get<JornadaResponseDto[]>(`${this.apiUrl}/ligas/${ligaId}/jornadas`);
  }

  obtenerResultadosPorLiga(ligaId: number): Observable<ResultadoPartidoResponseDto[]> {
    return this.http.get<ResultadoPartidoResponseDto[]>(`${this.apiUrl}/ligas/${ligaId}/resultados`);
  }

  obtenerClasificacionPorLiga(ligaId: number): Observable<EquipoClasificacionResponseDto[]> {
    return this.http.get<EquipoClasificacionResponseDto[]>(`${this.apiUrl}/ligas/${ligaId}/clasificacion`);

  }

  obtenerGoleadoresPorLiga(ligaId: number): Observable<GoleadorResponseDto[]> {
    return this.http.get<GoleadorResponseDto[]>(`${this.apiUrl}/ligas/${ligaId}/goleadores`);
  }

  buscarCompeticiones(
    temporada?: string,
    tipoJuego?: string,
    competicion?: string,
    grupo?: string
  ): Observable<any[]> {
    let params = new HttpParams();
    if (temporada) params = params.set('temporada', temporada);
    if (tipoJuego) params = params.set('tipoJuego', tipoJuego);
    if (competicion) params = params.set('competicion', competicion);
    if (grupo) params = params.set('grupo', grupo);

    return this.http.get<any[]>(`${this.apiUrl}/buscar`, { params });
  }

  obtenerEventosPartido(partidoId: number): Observable<any[]> {
    return this.http.get<any[]>(`http://localhost:5101/api/arbitro/partidos/${partidoId}/eventos`);
  }
}

