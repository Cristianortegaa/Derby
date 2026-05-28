export interface JornadaResponseDto {
  numero: number;
  partidos: PartidoResponseDto[];
}

export interface PartidoResponseDto {
  id: number;
  fechaHora: string;
  golesLocal: number;
  golesVisitante: number;
  estado: string;
  equipoLocal: EquipoResponseDto;
  equipoVisitante: EquipoResponseDto;
  arbitroNombre?: string;
}

export interface EquipoResponseDto {
  id: number;
  nombre: string;
  escudoUrl: string;
  sede: string;
}

export interface ResultadoPartidoResponseDto {
  id: number;
  equipoLocal: string;
  equipoVisitante: string;
  golesLocal: number;
  golesVisitante: number;
  fecha: string;
  escudoLocalUrl: string;
  escudoVisitanteUrl: string;
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

export interface EventoPartidoDto {
  id: number;
  minuto: number;
  tipoEvento: string;
  nombreJugador: string;
}
