import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-liga-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-liga-detail.html',
  styleUrl: './admin-liga-detail.css'
})
export class AdminLigaDetail implements OnInit {
  liga: any = null;
  ligaId: number = 0;
  cargando = true;
  generandoCalendario = false;

  equiposLiga: any[] = [];
  todosEquipos: any[] = [];
  equipoSeleccionado: number = 0;
  filtroEquipo = '';

  notificacion = { mostrar: false, tipo: 'exito' as 'exito' | 'error', mensaje: '' };
  modalConfirm = { mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Confirmar', tipo: 'danger' as 'danger' | 'success', onConfirm: () => {} };

  abrirConfirm(titulo: string, mensaje: string, accion: () => void, textoConfirmar = 'Confirmar', tipo: 'danger' | 'success' = 'danger') {
    this.modalConfirm = { mostrar: true, titulo, mensaje, textoConfirmar, tipo, onConfirm: accion };
  }
  cerrarConfirm() { this.modalConfirm.mostrar = false; }
  confirmar() { this.modalConfirm.onConfirm(); this.cerrarConfirm(); }

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.ligaId = Number(this.route.snapshot.paramMap.get('id'));
    this.cargarTodo();
  }

  cargarTodo() {
    this.cargando = true;
    Promise.all([
      this.adminService.obtenerLigas().toPromise(),
      this.adminService.obtenerEquiposSinLiga().toPromise(),
      this.adminService.obtenerEquiposLiga(this.ligaId).toPromise()
    ]).then(([ligas, equipos, equiposLiga]) => {
      this.liga = (ligas || []).find((l: any) => l.id === this.ligaId) || null;
      this.todosEquipos = equipos || [];
      this.equiposLiga = equiposLiga || [];
      this.cargando = false;
      this.cdr.detectChanges();
    }).catch(() => {
      this.cargando = false;
      this.cdr.detectChanges();
    });
  }

  get equiposDisponibles(): any[] {
    const idsEnLiga = new Set(this.equiposLiga.map((e: any) => e.id));
    return this.todosEquipos.filter((e: any) => !idsEnLiga.has(e.id));
  }

  get equiposFiltrados(): any[] {
    if (!this.filtroEquipo.trim()) return this.equiposDisponibles;
    return this.equiposDisponibles.filter((e: any) =>
      e.nombre.toLowerCase().includes(this.filtroEquipo.toLowerCase())
    );
  }

  agregarEquipo() {
    if (!this.equipoSeleccionado) return;
    this.adminService.añadirEquipoLiga(this.ligaId, this.equipoSeleccionado).subscribe({
      next: () => {
        this.equipoSeleccionado = 0;
        this.mostrarNotificacion('exito', 'Equipo añadido');
        this.cargarTodo();
      },
      error: (err: any) => this.mostrarNotificacion('error', err?.error?.error || 'Error al añadir equipo')
    });
  }

  quitarEquipo(equipoId: number) {
    this.abrirConfirm('Quitar equipo', '¿Quitar este equipo de la liga?', () => {
      this.adminService.quitarEquipoLiga(this.ligaId, equipoId).subscribe({
        next: () => {
          this.mostrarNotificacion('exito', 'Equipo eliminado');
          this.cargarTodo();
        },
        error: () => this.mostrarNotificacion('error', 'Error al quitar equipo')
      });
    }, 'Quitar');
  }

  mostrarNotificacion(tipo: 'exito' | 'error', mensaje: string) {
    this.notificacion = { mostrar: true, tipo, mensaje };
    setTimeout(() => { this.notificacion.mostrar = false; this.cdr.detectChanges(); }, 3000);
  }

  getBadgeClase(estado: string): string {
    const mapa: Record<string, string> = {
      'Activo': 'badge-activo', 'Inactivo': 'badge-inactivo',
      'Pausado': 'badge-pausado', 'Finalizado': 'badge-finalizado'
    };
    return mapa[estado] || 'badge-inactivo';
  }

  generarCalendario() {
    this.abrirConfirm('Generar Calendario', '¿Generar el calendario? Esta acción no se puede deshacer.', () => {
      this.generandoCalendario = true;
      this.adminService.generarCalendario(this.ligaId).subscribe({
        next: () => {
          this.generandoCalendario = false;
          this.mostrarNotificacion('exito', 'Calendario generado correctamente');
          this.cargarTodo();
        },
        error: (err: any) => {
          this.generandoCalendario = false;
          this.mostrarNotificacion('error', err?.error?.error || 'Error al generar calendario');
        }
      });
    }, 'Generar', 'success');
  }
}
