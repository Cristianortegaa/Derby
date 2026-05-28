import {Component, OnInit, ChangeDetectorRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {NavbarComponent} from '../../../navbar/navbar.component';
import {AdminService} from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-equipos',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-equipos.html',
  styleUrl: './admin-equipos.css'
})
export class AdminEquipos implements OnInit {
  equipos: any[] = [];
  equiposFiltrados: any[] = [];
  busqueda = '';
  filtroLiga = '';
  cargando = false;
  mostrarForm = false;
  formulario = {nombre: '', sede: '', escudoUrl: '', entrenador: ''};

  editandoId: number | null = null;
  mostrarFormEditar = false;
  formularioEditar = {nombre: '', sede: '', escudoUrl: '', entrenador: ''};

  jugadoresForm: { [equipoId: number]: { nombre: string, dorsal: number | null }[] } = {};
  notificacion = {mostrar: false, tipo: 'exito' as 'exito' | 'error', mensaje: ''};
  modalConfirm = {
    mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Eliminar', onConfirm: () => {
    }
  };

  constructor(private adminService: AdminService, private cdr: ChangeDetectorRef) {
  }

  ngOnInit() {
    this.cargarEquipos();
  }

  cargarEquipos() {
    this.cargando = true;
    this.adminService.obtenerEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
        this.aplicarFiltro();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  agregarEquipo() {
    if (!this.formulario.nombre || !this.formulario.sede) {
      this.mostrarNotificacion('error', 'Nombre y sede son obligatorios');
      return;
    }

    const jugadores = this.jugadoresForm[0] || [];
    if (jugadores.length < 13) {
      this.mostrarNotificacion('error', 'El equipo debe tener mínimo 13 jugadores');
      return;
    }

    this.adminService.crearEquipo(this.formulario).subscribe({
      next: (equipo: any) => {
        const promesas = jugadores
          .filter(j => j.nombre && j.dorsal)
          .map(j => this.adminService.agregarJugador(equipo.id, {nombre: j.nombre, dorsal: j.dorsal}).toPromise());
        Promise.all(promesas).then(() => {
          this.cargarEquipos();
          this.resetFormulario();
          this.mostrarForm = false;
          this.mostrarNotificacion('exito', 'Equipo creado correctamente');
        });
      },
      error: (err: any) => this.mostrarNotificacion('error', err?.error?.error || 'Error al crear equipo')
    });
  }

  abrirEditar(equipo: any) {
    this.editandoId = equipo.id;
    this.formularioEditar = {nombre: equipo.nombre, sede: equipo.sede, escudoUrl: equipo.escudoUrl || '', entrenador: equipo.entrenador || ''};
    this.mostrarFormEditar = true;
    setTimeout(() => {
      document.documentElement.scrollTop = 0;
      document.body.scrollTop = 0;
    }, 50);
  }

  cerrarEditar() {
    this.editandoId = null;
    this.mostrarFormEditar = false;
    this.formularioEditar = {nombre: '', sede: '', escudoUrl: '', entrenador: ''};
  }

  actualizarEquipo() {
    if (!this.formularioEditar.nombre || !this.formularioEditar.sede) {
      this.mostrarNotificacion('error', 'Nombre y sede son obligatorios');
      return;
    }
    this.adminService.actualizarEquipo(this.editandoId!, this.formularioEditar).subscribe({
      next: () => {
        this.cargarEquipos();
        this.cerrarEditar();
        this.mostrarNotificacion('exito', 'Equipo actualizado correctamente');
      },
      error: (err: any) => this.mostrarNotificacion('error', err?.error?.error || 'Error al actualizar equipo')
    });
  }

  abrirConfirm(titulo: string, mensaje: string, accion: () => void, textoConfirmar = 'Eliminar') {
    this.modalConfirm = {mostrar: true, titulo, mensaje, textoConfirmar, onConfirm: accion};
  }

  cerrarConfirm() {
    this.modalConfirm.mostrar = false;
  }

  confirmar() {
    this.modalConfirm.onConfirm();
    this.cerrarConfirm();
  }

  eliminarEquipo(id: number) {
    this.abrirConfirm('Eliminar equipo', '¿Estás seguro de eliminar este equipo?', () => {
      this.adminService.eliminarEquipo(id).subscribe({
        next: () => {
          this.cargarEquipos();
          this.mostrarNotificacion('exito', 'Equipo eliminado');
        },
        error: () => this.mostrarNotificacion('error', 'Error al eliminar equipo')
      });
    });
  }

  aplicarFiltro() {
    let resultado = this.equipos;
    if (this.busqueda.trim()) {
      const q = this.busqueda.toLowerCase();
      resultado = resultado.filter(e => e.nombre.toLowerCase().includes(q));
    }
    if (this.filtroLiga) {
      resultado = resultado.filter(e => e.ligaNombre === this.filtroLiga);
    }
    this.equiposFiltrados = resultado.sort((a, b) => a.nombre.localeCompare(b.nombre));
  }

  get ligas(): string[] {
    return [...new Set(this.equipos.map((e: any) => e.ligaNombre).filter((l: any) => l && l !== 'Sin Liga'))] as string[];
  }

  get jugadoresNuevos(): { nombre: string, dorsal: number | null }[] {
    if (!this.jugadoresForm[0]) this.jugadoresForm[0] = [{nombre: '', dorsal: null}];
    return this.jugadoresForm[0];
  }

  abrirForm(equipo?: any) {
    if (equipo) {
      this.editandoId = equipo.id;
      this.formulario = {nombre: equipo.nombre, sede: equipo.sede, escudoUrl: equipo.escudoUrl || '', entrenador: equipo.entrenador || ''};
    } else {
      this.editandoId = null;
      this.resetForm();
    }
    this.mostrarForm = true;
    setTimeout(() => { document.documentElement.scrollTop = 0; document.body.scrollTop = 0; }, 50);
  }

  resetForm() {
    this.formulario = {nombre: '', sede: '', escudoUrl: '', entrenador: ''};
  }

  agregarJugadorForm() {
    const ultimo = this.jugadoresNuevos[this.jugadoresNuevos.length - 1];
    if (!ultimo.nombre || !ultimo.dorsal) {
      this.mostrarNotificacion('error', 'Completa el jugador anterior antes de añadir otro');
      return;
    }
    if (this.jugadoresNuevos.length >= 25) {
      this.mostrarNotificacion('error', 'Máximo 25 jugadores');
      return;
    }
    this.jugadoresNuevos.push({nombre: '', dorsal: null});
  }

  quitarJugador(index: number) {
    this.jugadoresNuevos.splice(index, 1);
  }

  onEscudoSeleccionado(event: any, destino: 'nuevo' | 'editar') {
    const file = event.target.files[0];
    if (!file) {
      return;
    }
    const reader = new FileReader();
    reader.onload = (e: any) => {
      setTimeout(() => {
        if (destino === 'nuevo') {
          this.formulario.escudoUrl = e.target.result;
        } else {
          this.formularioEditar.escudoUrl = e.target.result;
        }
        this.cdr.detectChanges();
      }, 0);
    };
    reader.readAsDataURL(file);
  }

  resetFormulario() {
    this.formulario = {nombre: '', sede: '', escudoUrl: '', entrenador: ''};
    this.jugadoresForm[0] = [{nombre: '', dorsal: null}];
  }

  mostrarNotificacion(tipo: 'exito' | 'error', mensaje: string) {
    this.notificacion = {mostrar: true, tipo, mensaje};
    setTimeout(() => {
      this.notificacion.mostrar = false;
    }, 3000);
  }
}
