import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-ligas',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-ligas.html',
  styleUrl: './admin-ligas.css'
})
export class AdminLigas implements OnInit {

  ligas: any[] = [];
  ligasFiltradas: any[] = [];
  cargando = false;

  competiciones: any[] = [];

  busquedaNombre = '';
  filtroCompeticion = '';
  filtroEstado = '';

  competicionIdFijado: number | null = null;

  mostrarForm = false;
  editandoId: number | null = null;

  estados = ['Activo', 'Inactivo', 'Pausado', 'Finalizado'];
  grupos = ['Único', 'Grupo A', 'Grupo B', 'Grupo C', 'Grupo D'];

  formulario = {
    nombre: '',
    competicionId: null as number | null,
    grupo: 'Único',
    jornadas: 38,
    jornadaActual: 0,
    estado: 'Activo'
  };

  notificacion = {
    mostrar: false,
    mensaje: '',
    tipo: 'exito' as 'exito' | 'error'
  };

  modalConfirm = {
    mostrar: false,
    titulo: '',
    mensaje: '',
    onConfirm: () => {}
  };

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit() {
    const cid = this.route.snapshot.queryParamMap.get('competicionId');
    if (cid) {
      this.competicionIdFijado = Number(cid);
      this.filtroCompeticion = cid;
    }
    this.cargarCompeticiones();
    this.cargarLigas();
  }

  // ─── Utilidades ──────────────────────────────────────────────────────────────

  mostrarAlerta(mensaje: string, tipo: 'exito' | 'error' = 'exito') {
    this.notificacion = { mostrar: true, mensaje, tipo };
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

  getProgreso(liga: any): number {
    if (!liga.totalPartidos) return 0;
    return Math.round((liga.partidosFinalizados / liga.totalPartidos) * 100);
  }

  getNombreComp(competicionId: number): string {
    return this.competiciones.find(c => c.id === competicionId)?.nombre || '—';
  }

  get statActivas(): number {
    return this.ligas.filter(l => l.estado === 'Activo').length;
  }

  get statTotalEquipos(): number {
    return this.ligas.reduce((acc, l) => acc + (l.equipos || 0), 0);
  }

  get progresoMedio(): number {
    if (!this.ligas.length) return 0;
    const suma = this.ligas.reduce((acc, l) => acc + this.getProgreso(l), 0);
    return Math.round(suma / this.ligas.length);
  }

  // ─── Modal confirmación ──────────────────────────────────────────────────────

  abrirConfirm(titulo: string, mensaje: string, accion: () => void) {
    this.modalConfirm = { mostrar: true, titulo, mensaje, onConfirm: accion };
  }

  cerrarConfirm() {
    this.modalConfirm.mostrar = false;
  }

  confirmar() {
    this.modalConfirm.onConfirm();
    this.cerrarConfirm();
  }

  // ─── Carga ───────────────────────────────────────────────────────────────────

  cargarLigas() {
    this.cargando = true;
    this.adminService.obtenerLigas().subscribe({
      next: (data) => {
        this.ligas = data;
        this.aplicarFiltro();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error cargando ligas:', error);
        this.mostrarAlerta('Error al cargar ligas', 'error');
        this.ligas = [];
        this.ligasFiltradas = [];
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  cargarCompeticiones() {
    this.adminService.obtenerCompeticiones().subscribe({
      next: (data) => {
        this.competiciones = data;
        if (!this.formulario.competicionId && data.length > 0) {
          this.formulario.competicionId = data[0].id;
        }
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.warn('Error cargando competiciones:', error);
      }
    });
  }

  // ─── Filtros ─────────────────────────────────────────────────────────────────

  aplicarFiltro() {
    let resultado = this.ligas;

    if (this.busquedaNombre.trim()) {
      const q = this.busquedaNombre.toLowerCase();
      resultado = resultado.filter(l =>
        l.nombre.toLowerCase().includes(q) ||
        this.getNombreComp(l.competicionId).toLowerCase().includes(q) ||
        l.grupo?.toLowerCase().includes(q)
      );
    }

    if (this.filtroCompeticion) {
      resultado = resultado.filter(l => String(l.competicionId) === this.filtroCompeticion);
    }

    if (this.filtroEstado) {
      resultado = resultado.filter(l => l.estado === this.filtroEstado);
    }

    this.ligasFiltradas = resultado;
    this.cdr.detectChanges();
  }

  limpiarFiltros() {
    this.busquedaNombre = '';
    if (!this.competicionIdFijado) this.filtroCompeticion = '';
    this.filtroEstado = '';
    this.aplicarFiltro();
  }

  // ─── CRUD ────────────────────────────────────────────────────────────────────

  abrirForm(liga?: any) {
    if (liga) {
      this.editandoId = liga.id;
      this.formulario = {
        nombre: liga.nombre,
        competicionId: liga.competicionId,
        grupo: liga.grupo || 'Único',
        jornadas: liga.jornadas,
        jornadaActual: liga.jornadaActual || 0,
        estado: liga.estado
      };
    } else {
      this.editandoId = null;
      this.resetForm();
    }
    this.mostrarForm = true;
    setTimeout(() => { document.documentElement.scrollTop = 0; document.body.scrollTop = 0; }, 50);
  }

  cerrarForm() {
    this.mostrarForm = false;
    this.editandoId = null;
    this.resetForm();
  }

  resetForm() {
    this.formulario = {
      nombre: '',
      competicionId: this.competiciones[0]?.id || null,
      grupo: 'Único',
      jornadas: 38,
      jornadaActual: 0,
      estado: 'Activo'
    };
  }

  guardar() {
    if (!this.formulario.nombre || !this.formulario.competicionId) {
      this.mostrarAlerta('Nombre y competición son requeridos', 'error');
      return;
    }
    if (this.formulario.jornadaActual > this.formulario.jornadas) {
      this.mostrarAlerta('La jornada actual no puede superar el total de jornadas', 'error');
      return;
    }

    if (this.editandoId === null) {
      this.adminService.crearLiga(this.formulario).subscribe({
        next: () => {
          this.cerrarForm();
          this.cargarLigas();
          this.mostrarAlerta('¡Liga creada correctamente!');
        },
        error: (e) => this.mostrarAlerta('Error: ' + (e?.error?.error || e?.message), 'error')
      });
    } else {
      this.adminService.actualizarLiga(this.editandoId, this.formulario).subscribe({
        next: () => {
          this.cerrarForm();
          this.cargarLigas();
          this.mostrarAlerta('Liga actualizada correctamente');
        },
        error: (e) => this.mostrarAlerta('Error: ' + (e?.error?.error || e?.message), 'error')
      });
    }
  }

  eliminar(id: number, nombre: string) {
    this.abrirConfirm(
      'Eliminar liga',
      `¿Estás seguro de que quieres eliminar la liga "${nombre}"? Esta acción no se puede deshacer.`,
      () => {
        this.adminService.eliminarLiga(id).subscribe({
          next: () => {
            this.cargarLigas();
            this.mostrarAlerta('Liga eliminada del sistema');
          },
          error: () => this.mostrarAlerta('Error al eliminar la liga', 'error')
        });
      }
    );
  }
}
