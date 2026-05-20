import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-equipo-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-equipo-detail.html',
  styleUrl: './admin-equipo-detail.css'
})
export class AdminEquipoDetail implements OnInit {
  equipoId: number = 0;
  equipo: any = null;
  jugadores: any[] = [];
  cargando = true;

  editandoEquipo = false;
  formularioEquipo = { nombre: '', sede: '', entrenador: '' };

  formularioJugador = { nombre: '', dorsal: null as number | null };
  editandoJugadorId: number | null = null;
  mostrarFormJugador = false;

  notificacion = { mostrar: false, tipo: 'exito' as 'exito' | 'error', mensaje: '' };
  modalConfirm = { mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Eliminar', onConfirm: () => {} };

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.equipoId = Number(this.route.snapshot.paramMap.get('id'));
    this.cargarTodo();
  }

  cargarTodo() {
    this.cargando = true;
    Promise.all([
      this.adminService.obtenerEquipos().toPromise(),
      this.adminService.obtenerJugadores(this.equipoId).toPromise()
    ]).then(([equipos, jugadores]) => {
      this.equipo = (equipos || []).find((e: any) => e.id === this.equipoId) || null;
      this.jugadores = jugadores || [];
      this.cargando = false;
      this.cdr.detectChanges();
    }).catch(() => {
      this.cargando = false;
      this.cdr.detectChanges();
    });
  }

  abrirEditarEquipo() {
    this.formularioEquipo = { nombre: this.equipo.nombre, sede: this.equipo.sede, entrenador: this.equipo.entrenador || '' };
    this.editandoEquipo = true;
  }

  cerrarEditarEquipo() {
    this.editandoEquipo = false;
  }

  guardarEquipo() {
    this.adminService.actualizarEquipo(this.equipoId, this.formularioEquipo).subscribe({
      next: () => {
        this.mostrarNotificacion('exito', 'Equipo actualizado');
        this.cerrarEditarEquipo();
        this.cargarTodo();
      },
      error: (err: any) => this.mostrarNotificacion('error', err?.error?.error || 'Error al actualizar')
    });
  }

  abrirFormJugador(jugador?: any) {
    if (jugador) {
      this.editandoJugadorId = jugador.id;
      this.formularioJugador = { nombre: jugador.nombre, dorsal: jugador.dorsal };
    } else {
      this.editandoJugadorId = null;
      this.formularioJugador = { nombre: '', dorsal: null };
    }
    this.mostrarFormJugador = true;
  }

  cerrarFormJugador() {
    this.mostrarFormJugador = false;
    this.editandoJugadorId = null;
    this.formularioJugador = { nombre: '', dorsal: null };
  }

  guardarJugador() {
    if (!this.formularioJugador.nombre || !this.formularioJugador.dorsal) {
      this.mostrarNotificacion('error', 'Nombre y dorsal son obligatorios');
      return;
    }
    if (this.editandoJugadorId) {
      this.adminService.actualizarJugador(this.editandoJugadorId, this.formularioJugador).subscribe({
        next: () => {
          this.mostrarNotificacion('exito', 'Jugador actualizado');
          this.cerrarFormJugador();
          this.cargarTodo();
        },
        error: (err: any) => this.mostrarNotificacion('error', err?.error?.error || 'Error al actualizar jugador')
      });
    } else {
      this.adminService.agregarJugador(this.equipoId, this.formularioJugador).subscribe({
        next: () => {
          this.mostrarNotificacion('exito', 'Jugador añadido');
          this.cerrarFormJugador();
          this.cargarTodo();
        },
        error: (err: any) => this.mostrarNotificacion('error', err?.error?.error || 'Error al añadir jugador')
      });
    }
  }

  eliminarJugador(id: number) {
    this.abrirConfirm('Eliminar jugador', '¿Eliminar este jugador del equipo?', () => {
      this.adminService.eliminarJugador(id).subscribe({
        next: () => {
          this.mostrarNotificacion('exito', 'Jugador eliminado');
          this.cargarTodo();
        },
        error: () => this.mostrarNotificacion('error', 'Error al eliminar jugador')
      });
    });
  }

  abrirConfirm(titulo: string, mensaje: string, accion: () => void, textoConfirmar = 'Eliminar') {
    this.modalConfirm = { mostrar: true, titulo, mensaje, textoConfirmar, onConfirm: accion };
  }
  cerrarConfirm() { this.modalConfirm.mostrar = false; }
  confirmar() { this.modalConfirm.onConfirm(); this.cerrarConfirm(); }

  mostrarNotificacion(tipo: 'exito' | 'error', mensaje: string) {
    this.notificacion = { mostrar: true, tipo, mensaje };
    setTimeout(() => { this.notificacion.mostrar = false; this.cdr.detectChanges(); }, 3000);
  }
}
