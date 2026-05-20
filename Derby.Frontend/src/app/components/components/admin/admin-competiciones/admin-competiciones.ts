import {Component, OnInit, ChangeDetectorRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {NavbarComponent} from '../../../navbar/navbar.component';
import {AdminService} from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-competiciones',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-competiciones.html',
  styleUrl: './admin-competiciones.css'
})
export class AdminCompeticiones implements OnInit {

  competiciones: any[] = [];
  competicionesFiltradas: any[] = [];
  cargando = false;

  busquedaNombre = '';
  filtroEstado = '';

  mostrarFormComp = false;
  editandoCompId: number | null = null;

  estados = ['Activo', 'Inactivo', 'Pausado', 'Finalizado'];
  tipos = ['Liga', 'Copa', 'Torneo', 'Supercopa'];
  tiposJuego = ['Futbol-11', 'Futbol-7', 'Futbol-Sala'];

  formularioComp = {
    nombre: '',
    descripcion: '',
    temporada: '',
    estado: 'Activo',
    tipo: 'Liga',
    tipoJuego: 'Futbol-11'
  };

  // ─── Notificación ────────────────────────────────────────────────────────────
  notificacion = {
    mostrar: false,
    mensaje: '',
    tipo: 'exito' as 'exito' | 'error'
  };

  modalConfirm = {
    mostrar: false,
    titulo: '',
    mensaje: '',
    onConfirm: () => {
    }
  };

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    this.cargarCompeticiones();
  }

  // ─── Utilidades ──────────────────────────────────────────────────────────────

  mostrarAlerta(mensaje: string, tipo: 'exito' | 'error' = 'exito') {
    this.notificacion = {mostrar: true, mensaje, tipo};
    this.cdr.detectChanges();
    setTimeout(() => {
      this.notificacion.mostrar = false;
      this.cdr.detectChanges();
    }, 3500);
  }

  getBadgeClase(estado: string): string {
    const mapa: Record<string, string> = {
      'Activo': 'badge-activo',
      'Inactivo': 'badge-inactivo',
      'Pausado': 'badge-pausado',
      'Finalizado': 'badge-finalizado'
    };
    return mapa[estado] || 'badge-inactivo';
  }

  get statActivas(): number {
    return this.competiciones.filter(c => c.estado === 'Activo').length;
  }

  // ─── Carga ───────────────────────────────────────────────────────────────────

  cargarCompeticiones() {
    this.cargando = true;
    this.adminService.obtenerCompeticiones().subscribe({
      next: (data) => {
        this.competiciones = data;
        this.aplicarFiltro();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error cargando competiciones:', error);
        this.mostrarAlerta('Error al cargar competiciones', 'error');
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  // ─── Filtros ─────────────────────────────────────────────────────────────────

  aplicarFiltro() {
    let resultado = this.competiciones;
    if (this.busquedaNombre.trim()) {
      const q = this.busquedaNombre.toLowerCase();
      resultado = resultado.filter(c =>
        c.nombre.toLowerCase().includes(q) ||
        c.temporada?.toLowerCase().includes(q)
      );
    }
    if (this.filtroEstado) {
      resultado = resultado.filter(c => c.estado === this.filtroEstado);
    }
    this.competicionesFiltradas = resultado;
    this.cdr.detectChanges();
  }

  // ─── CRUD ────────────────────────────────────────────────────────────────────

  abrirFormComp(comp?: any) {
    if (comp) {
      this.editandoCompId = comp.id;
      this.formularioComp = {
        nombre: comp.nombre,
        descripcion: comp.descripcion || '',
        temporada: comp.temporada,
        estado: comp.estado,
        tipo: comp.tipo || 'Liga',
        tipoJuego: comp.tipoJuego || 'Futbol-11'
      };
    } else {
      this.editandoCompId = null;
      this.resetForm();
    }
    this.mostrarFormComp = true;
  }

  cerrarForm() {
    this.mostrarFormComp = false;
    this.editandoCompId = null;
    this.resetForm();
  }

  resetForm() {
    this.formularioComp = {
      nombre: '',
      descripcion: '',
      temporada: '',
      estado: 'Activo',
      tipo: 'Liga',
      tipoJuego: 'Futbol-11'
    };
  }

  guardarComp() {
    if (!this.formularioComp.nombre || !this.formularioComp.temporada) {
      this.mostrarAlerta('Nombre y temporada son requeridos', 'error');
      return;
    }
    if (this.editandoCompId === null) {
      this.adminService.crearCompeticion(this.formularioComp).subscribe({
        next: () => {
          this.cerrarForm();
          this.cargarCompeticiones();
          this.mostrarAlerta('¡Competición creada correctamente!');
        },
        error: (e) => this.mostrarAlerta('Error: ' + (e?.error?.error || e?.message), 'error')
      });
    } else {
      this.adminService.actualizarCompeticion(this.editandoCompId, this.formularioComp).subscribe({
        next: () => {
          this.cerrarForm();
          this.cargarCompeticiones();
          this.mostrarAlerta('Competición actualizada correctamente');
        },
        error: (e) => this.mostrarAlerta('Error: ' + (e?.error?.error || e?.message), 'error')
      });
    }
  }

  eliminarComp(id: number, nombre: string) {
    this.abrirConfirm(
      'Eliminar competición',
      `¿Estás seguro de que quieres eliminar "${nombre}"? También se eliminarán sus ligas.`,
      () => {
        this.adminService.eliminarCompeticion(id).subscribe({
          next: () => {
            this.cargarCompeticiones();
            this.mostrarAlerta('Competición eliminada del sistema');
          },
          error: () => this.mostrarAlerta('Error al eliminar la competición', 'error')
        });
      }
    );
  }

  abrirConfirm(titulo: string, mensaje: string, accion: () => void) {
    this.modalConfirm = {
      mostrar: true, titulo, mensaje, onConfirm: accion
    };
  }

  cerrarConfirm() {
    this.modalConfirm.mostrar = false;
  }

  confirmar() {
    this.modalConfirm.onConfirm();
    this.cerrarConfirm();
  }

}
