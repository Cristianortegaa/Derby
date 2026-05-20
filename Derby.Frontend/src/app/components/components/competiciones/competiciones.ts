import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../navbar/navbar.component';
import { CompeticionesService } from '../../../services/competiciones.service';

interface Equipo {
  id: number;
  nombre: string;
  escudoUrl?: string;
  sede?: string;
  division?: string;
}

interface Resultado {
  local: number;
  visitante: number;
}

interface Partido {
  id: number;
  equipoLocal: Equipo;
  equipoVisitante: Equipo;
  resultado: Resultado;
  estado: 'jugado' | 'en-juego' | 'pendiente';
  fecha: string;
}

interface Jornada {
  numero: number;
  partidos: Partido[];
}

interface Filtros {
  temporada: string;
  tipoJuego: string;
  competicion: string;
  grupo: string;
}

interface EquipoClasificacion {
  id: number;
  nombre: string;
  pj: number;
  g: number;
  e: number;
  p: number;
  gf: number;
  gc: number;
  pts: number;
}

interface ResultadoPartido {
  id: number;
  equipoLocal: string;
  equipoVisitante: string;
  golesLocal: number;
  golesVisitante: number;
  fecha: string;
}

interface Goleador {
  id: number;
  nombre: string;
  equipo: string;
  goles: number;
}

type TabActiva = 'calendario' | 'clasificacion' | 'resultados' | 'goleadores';

@Component({
  selector: 'app-competiciones',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FormsModule],
  templateUrl: './competiciones.html',
  styleUrl: './competiciones.css'
})
export class Competiciones implements OnInit {
  filtros: Filtros = {
    temporada: '',
    tipoJuego: '',
    competicion: '',
    grupo: ''
  };

  tabActiva: TabActiva = 'calendario';
  jornadas: Jornada[] = [];
  clasificacion: EquipoClasificacion[] = [];
  goleadores: Goleador[] = [];
  tituloCalendario: string = 'Calendario de Partidos';
  jornadaSeleccionada: number = 1;
  cargando: boolean = false;
  error: string = '';

  private apiUrl = 'http://localhost:5297/api/competiciones';
  private competicionIdActual: number = 1; // Deberá obtenerse dinámicamente

  constructor(private competicionesService: CompeticionesService) {}

  ngOnInit(): void {
    // Inicializar si es necesario
  }

  get resultados(): ResultadoPartido[] {
    const resultadosList: ResultadoPartido[] = [];

    this.jornadas.forEach(jornada => {
      jornada.partidos.forEach(partido => {
        if (partido.estado === 'jugado') {
          resultadosList.push({
            id: partido.id,
            equipoLocal: partido.equipoLocal.nombre,
            equipoVisitante: partido.equipoVisitante.nombre,
            golesLocal: partido.resultado.local,
            golesVisitante: partido.resultado.visitante,
            fecha: partido.fecha
          });
        }
      });
    });

    return resultadosList;
  }

  cambiarTab(tab: TabActiva): void {
    this.tabActiva = tab;
  }

  seleccionarJornada(numero: number): void {
    this.jornadaSeleccionada = numero;
  }

  obtenerPartidosJornada(numero: number): Partido[] {
    const jornada = this.jornadas.find(j => j.numero === numero);
    return jornada ? jornada.partidos : [];
  }

  async buscarCompeticiones(): Promise<void> {
    // Validar que los filtros estén completos
    if (!this.filtros.temporada || !this.filtros.tipoJuego || !this.filtros.competicion || !this.filtros.grupo) {
      this.error = 'Por favor, completa todos los filtros';
      return;
    }

    this.cargando = true;
    this.error = '';

    try {
      // Generar título dinámico
      this.tituloCalendario = `${this.filtros.temporada} - ${this.filtros.competicion} ${this.filtros.grupo}`;

      // Llamar al servicio para obtener jornadas
      const jornadasResponse = await this.competicionesService.buscarCompeticiones(
        this.filtros.temporada,
        this.filtros.tipoJuego,
        this.filtros.competicion,
        this.filtros.grupo
      ).toPromise();

      // Convertir DTOs del backend a interfaces locales
      this.jornadas = (jornadasResponse || []).map((jornada: any) => ({
        numero: jornada.numero,
        partidos: jornada.partidos.map((partido: any) => ({
          id: partido.id,
          equipoLocal: {
            id: partido.equipoLocal.id,
            nombre: partido.equipoLocal.nombre,
            escudoUrl: partido.equipoLocal.escudoUrl
          },
          equipoVisitante: {
            id: partido.equipoVisitante.id,
            nombre: partido.equipoVisitante.nombre,
            escudoUrl: partido.equipoVisitante.escudoUrl
          },
          resultado: {
            local: partido.golesLocal,
            visitante: partido.golesVisitantes
          },
          estado: partido.estado as 'jugado' | 'en-juego' | 'pendiente',
          fecha: partido.fecha
        }))
      }));

      // Obtener clasificación
      await this.cargarClasificacion();

      // Obtener goleadores
      await this.cargarGoleadores();

      // Resetear a la primera jornada
      this.jornadaSeleccionada = 1;
    } catch (err) {
      console.error('Error al buscar competiciones:', err);
      this.error = 'Error al cargar los datos. Intenta nuevamente.';
    } finally {
      this.cargando = false;
    }
  }

  private async cargarClasificacion(): Promise<void> {
    try {
      const clasificacionResponse = await this.competicionesService.obtenerClasificacion(this.competicionIdActual).toPromise();

      this.clasificacion = (clasificacionResponse || []).map((equipo: any) => ({
        id: equipo.id,
        nombre: equipo.nombre,
        pj: equipo.partidosJugados,
        g: equipo.ganancias,
        e: equipo.empates,
        p: equipo.derrotas,
        gf: equipo.golesAFavor,
        gc: equipo.golesEnContra,
        pts: equipo.puntos
      }));
    } catch (err) {
      console.error('Error al cargar clasificación:', err);
    }
  }

  private async cargarGoleadores(): Promise<void> {
    try {
      const goleadoresResponse = await this.competicionesService.obtenerGoleadores(this.competicionIdActual).toPromise();

      this.goleadores = goleadoresResponse || [];
    } catch (err) {
      console.error('Error al cargar goleadores:', err);
    }
  }

  limpiarFiltros(): void {
    this.filtros = {
      temporada: '',
      tipoJuego: '',
      competicion: '',
      grupo: ''
    };
    this.jornadas = [];
    this.clasificacion = [];
    this.goleadores = [];
    this.tituloCalendario = 'Calendario de Partidos';
    this.tabActiva = 'calendario';
    this.error = '';
  }
}

