import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-partidos',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-partidos.html',
  styleUrls: ['./admin-partidos.css']
})
export class AdminPartidosComponent implements OnInit {
  partidos: any[] = [];
  partidosFiltrados: any[] = [];
  ligas: any[] = [];
  equipos: any[] = [];
  arbitros: any[] = [];

  cargando = false;
  mostrarForm = false;
  editandoId: number | null = null;

  filtroLiga: string | number = '';
  filtroEstado = '';
  filtroJornada: string | number = '';

  competicionIdFijado: number | null = null;
  ligaIdFijado: number | null = null;

  formulario: any = this.nuevoPartido();

  notificacion = {
    mostrar: false,
    tipo: 'exito' as 'exito' | 'error',
    mensaje: ''
  };

  modalConfirm = { mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Eliminar', onConfirm: () => {} };

  abrirConfirm(titulo: string, mensaje: string, accion: () => void, textoConfirmar = 'Eliminar') {
    this.modalConfirm = { mostrar: true, titulo, mensaje, textoConfirmar, onConfirm: accion };
  }
  cerrarConfirm() { this.modalConfirm.mostrar = false; }
  confirmar() { this.modalConfirm.onConfirm(); this.cerrarConfirm(); }

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const cid = this.route.snapshot.queryParams['competicionId'];
    if (cid) this.competicionIdFijado = Number(cid);
    const lid = this.route.snapshot.queryParams['ligaId'];
    if (lid) this.ligaIdFijado = Number(lid);
    this.cargarDatos();
  }

  async cargarDatos(): Promise<void> {
    this.cargando = true;
    try {
      const [ligas, equipos, arbitros, partidos] = await Promise.all([
        this.adminService.obtenerLigas().toPromise(),
        this.adminService.obtenerEquipos().toPromise(),
        this.adminService.obtenerArbitros().toPromise(),
        this.adminService.obtenerPartidos().toPromise()
      ]);

      const todasLigas = ligas || [];
      this.ligas = this.ligaIdFijado
        ? todasLigas.filter((l: any) => l.id === this.ligaIdFijado)
        : this.competicionIdFijado
          ? todasLigas.filter((l: any) => l.competicionId === this.competicionIdFijado)
          : todasLigas;
      this.equipos = equipos || [];
      this.arbitros = arbitros || [];
      this.partidos = partidos || [];
      if (this.ligaIdFijado) this.filtroLiga = this.ligaIdFijado;
      this.aplicarFiltros();
    } catch (error) {
      this.mostrarNotificacion('error', 'Error al cargar datos');
      console.error(error);
    } finally {
      this.cargando = false;
      this.cdr.detectChanges();
    }
  }

  aplicarFiltros(): void {
    this.partidosFiltrados = this.partidos.filter(p => {
      const ligaId = this.filtroLiga ? Number(this.filtroLiga) : 0;
      const jornada = this.filtroJornada ? Number(this.filtroJornada) : 0;
      const matchLiga = !this.filtroLiga || p.ligaId === ligaId;
      const matchEstado = !this.filtroEstado || p.estado === this.filtroEstado;
      const matchJornada = !this.filtroJornada || p.jornada === jornada;
      return matchLiga && matchEstado && matchJornada;
    });
  }

  abrirForm(partido?: any): void {
    if (partido) {
      this.editandoId = partido.id;
      this.formulario = { ...partido };
    } else {
      this.editandoId = null;
      this.formulario = this.nuevoPartido();
    }
    this.mostrarForm = true;
  }

  cerrarForm(): void {
    this.mostrarForm = false;
    this.editandoId = null;
    this.formulario = this.nuevoPartido();
  }

  async guardar(): Promise<void> {
    if (!this.formulario.ligaId || !this.formulario.equipoLocalId || !this.formulario.equipoVisitanteId) {
      this.mostrarNotificacion('error', 'Por favor completa todos los campos requeridos');
      return;
    }

    try {
      if (this.editandoId) {
        await this.adminService.actualizarPartido(this.editandoId, this.formulario).toPromise();
        this.mostrarNotificacion('exito', 'Partido actualizado correctamente');
      } else {
        await this.adminService.crearPartido(this.formulario).toPromise();
        this.mostrarNotificacion('exito', 'Partido creado correctamente');
      }
      this.cerrarForm();
      await this.cargarDatos();
    } catch (error: any) {
      this.mostrarNotificacion('error', error?.error?.error || 'Error al guardar partido');
      console.error(error);
    }
  }

  eliminar(id: number, descripcion: string): void {
    this.abrirConfirm('Eliminar partido', `¿Eliminar el partido "${descripcion}"?`, async () => {
      try {
        await this.adminService.eliminarPartido(id).toPromise();
        this.mostrarNotificacion('exito', 'Partido eliminado correctamente');
        await this.cargarDatos();
      } catch (error) {
        this.mostrarNotificacion('error', 'Error al eliminar partido');
        console.error(error);
      }
    });
  }

  nuevoPartido(): any {
    return {
      id: undefined,
      jornada: 1,
      ligaId: 0,
      equipoLocalId: 0,
      equipoVisitanteId: 0,
      golesLocal: 0,
      golesVisitante: 0,
      estado: 'Pendiente',
      fechaHora: undefined,
      arbitroId: undefined
    };
  }

  obtenerDescripcionPartido(partido: any): string {
    const local = partido.equipoLocal?.nombre || 'Equipo Local';
    const visitante = partido.equipoVisitante?.nombre || 'Equipo Visitante';
    return `${local} vs ${visitante}`;
  }

  mostrarNotificacion(tipo: 'exito' | 'error', mensaje: string): void {
    this.notificacion = { mostrar: true, tipo, mensaje };
    setTimeout(() => {
      this.notificacion.mostrar = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  get jornadasDisponibles(): number[] {
    const max = Math.max(...this.partidos.map(p => p.jornada), 10);
    return Array.from({ length: max }, (_, i) => i + 1);
  }

  get partidosPendientes(): number {
    return this.partidos.filter(p => p.estado === 'Pendiente').length;
  }

  get partidosEnJuego(): number {
    return this.partidos.filter(p => p.estado === 'En juego').length;
  }

  get partidosFinalizados(): number {
    return this.partidos.filter(p => p.estado === 'Finalizado').length;
  }
}

