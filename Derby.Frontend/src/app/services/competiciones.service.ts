import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface JornadaResponseDto {
  numero: number;
  partidos: PartidoResponseDto[];
}

export interface PartidoResponseDto {
  id: number;
  fecha: string;
  golesLocal: number;
  golesVisitantes: number;
  estado: string;
  equipoLocal: EquipoResponseDto;
  equipoVisitante: EquipoResponseDto;
}

export interface EquipoResponseDto {
  id: number;
  nombre: string;
  escudoUrl: string;
  sede: string;
  division: string;
}

export interface ResultadoPartidoResponseDto {
  id: number;
  equipoLocal: string;
  equipoVisitante: string;
  golesLocal: number;
  golesVisitante: number;
  fecha: string;
}

export interface EquipoClasificacionResponseDto {
  id: number;
  nombre: string;
  partidosJugados: number;
  ganancias: number;
  empates: number;
  derrotas: number;
  golesAFavor: number;
  golesEnContra: number;
  puntos: number;
}

export interface GoleadorResponseDto {
  id: number;
  nombre: string;
  equipo: string;
  goles: number;
}

@Injectable({
  providedIn: 'root'
})
export class CompeticionesService {
  private apiUrl = 'http://localhost:5101/api/competiciones';

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
}

