import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../navbar/navbar.component';
import { CompeticionesService } from '../../../services/competiciones.service';
import { AdminService } from '../../../services/admin.service';
import { JornadaResponseDto, EquipoClasificacionResponseDto, ResultadoPartidoResponseDto, GoleadorResponseDto, EventoPartidoDto } from '../../../models/competicion.model';

type TabActiva = 'calendario' | 'clasificacion' | 'resultados' | 'goleadores';

@Component({
  selector: 'app-competiciones',
  standalone: true,
  imports: [CommonModule, NavbarComponent, FormsModule],
  templateUrl: './competiciones.html',
  styleUrl: './competiciones.css'
})
export class Competiciones implements OnInit {
  competiciones: any[] = [];
  ligas: any[] = [];
  todasLigas: any[] = [];
  competicionSeleccionada: number = 0;
  ligaSeleccionada: number = 0;

  tabActiva: TabActiva = 'calendario';
  jornadas: JornadaResponseDto[] = [];
  clasificacion: EquipoClasificacionResponseDto[] = [];
  resultados: ResultadoPartidoResponseDto[] = [];
  goleadores: GoleadorResponseDto[] = [];
  jornadaSeleccionada: number = 0;
  cargando: boolean = false;
  error: string = '';

  modalActaAbierto = false;
  partidoActa: any = null;
  eventosActa: EventoPartidoDto[] = [];
  cargandoActa = false;

  constructor(
    private competicionesService: CompeticionesService,
    private adminService: AdminService
  ) {}

  ngOnInit(): void {
    this.adminService.obtenerCompeticiones().subscribe({
      next: (competiciones) => {
        this.competiciones = competiciones;
        this.adminService.obtenerLigas().subscribe({
          next: (ligas) => {
            this.todasLigas = ligas;
          },
          error: () => {}
        });
      },
      error: () => { this.error = 'Error al cargar competiciones'; }
    });
  }

  cambiarCompeticion(): void {
    this.ligas = this.todasLigas.filter((l: any) => l.competicionId == Number(this.competicionSeleccionada));
    this.ligaSeleccionada = 0;
    this.jornadas = [];
    this.clasificacion = [];
    this.resultados = [];
    this.goleadores = [];
  }

  cambiarLiga(): void {
    if (!this.ligaSeleccionada) return;
    this.cargando = true;
    this.error = '';

    Promise.all([
      this.competicionesService.obtenerJornadasPorLiga(this.ligaSeleccionada).toPromise(),
      this.competicionesService.obtenerClasificacionPorLiga(this.ligaSeleccionada).toPromise(),
      this.competicionesService.obtenerResultadosPorLiga(this.ligaSeleccionada).toPromise(),
      this.competicionesService.obtenerGoleadoresPorLiga(this.ligaSeleccionada).toPromise()
    ]).then(([jornadas, clasificacion, resultados, goleadores]) => {
      this.jornadas = jornadas || [];
      this.clasificacion = clasificacion || [];
      this.resultados = resultados || [];
      this.goleadores = goleadores || [];
      this.jornadaSeleccionada = 0;
    }).catch((err) => {
      console.error(err);
      this.error = 'Error al cargar los datos de la liga.';
    }).finally(() => {
      this.cargando = false;
    });
  }

  cambiarTab(tab: TabActiva): void {
    this.tabActiva = tab;
  }

  verActa(partido: any): void {
    this.partidoActa = partido;
    this.eventosActa = [];
    this.modalActaAbierto = true;
    this.cargandoActa = true;
    this.competicionesService.obtenerEventosPartido(partido.id).subscribe({
      next: (data) => {
        this.eventosActa = data;
        this.cargandoActa = false;
      },
      error: () => { this.cargandoActa = false; }
    });
  }

  cerrarActa(): void {
    this.modalActaAbierto = false;
    this.partidoActa = null;
    this.eventosActa = [];
  }
}

