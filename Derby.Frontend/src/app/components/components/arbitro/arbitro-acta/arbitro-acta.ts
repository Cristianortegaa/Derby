import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute, Router } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { ArbitroService } from '../../../../services/arbitro.service';
import { AdminService } from '../../../../services/admin.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-arbitro-acta',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './arbitro-acta.html',
  styleUrl: './arbitro-acta.css'
})
export class AbitroActa implements OnInit {
  partido: any = null;
  jugadores: any[] = [];
  eventos: any[] = [];
  cargando = false;
  cargandoEvento = false;
  partidoId: number = 0;

  nuevoEvento = {
    jugadorId: 0,
    minuto: 0,
    tipoEvento: 'Gol'
  };

  dropdownAbierto = false;
  jugadorSeleccionadoNombre = 'Selecciona jugador';
  modalConfirm = { mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Cerrar Acta', onConfirm: () => {} };

  abrirConfirm(titulo: string, mensaje: string, textoConfirmar: string, accion: () => void) {
    this.modalConfirm = { mostrar: true, titulo, mensaje, textoConfirmar, onConfirm: accion };
  }
  cerrarConfirm() { this.modalConfirm.mostrar = false; }
  confirmar() { this.modalConfirm.onConfirm(); this.cerrarConfirm(); }

  seleccionarJugador(id: number, nombre: string) {
    this.nuevoEvento.jugadorId = id;
    this.jugadorSeleccionadoNombre = nombre;
    this.dropdownAbierto = false;
    this.cdr.detectChanges();
  }

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private arbitroService: ArbitroService,
    private adminService: AdminService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.partidoId = Number(this.route.snapshot.paramMap.get('partidoId'));
    const usuario = this.authService.obtenerUsuarioActual();
    if (usuario) {
      this.cargarPartido(usuario.arbitroId ?? 0);
    }
  }

  cargarPartido(arbitroId: number) {
    this.cargando = true;
    this.arbitroService.obtenerMisPartidos(arbitroId).subscribe({
      next: (partidos) => {
        this.partido = partidos.find((p: any) => p.id === this.partidoId);
        if (this.partido) {
          this.cargarJugadores();
          this.cargarEventos();
        }
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: () => { this.cargando = false; this.cdr.detectChanges(); }
    });
  }

  cargarJugadores() {
    this.adminService.obtenerJugadores(this.partido.equipoLocalId).subscribe({
      next: (jugadoresLocal) => {
        this.adminService.obtenerJugadores(this.partido.equipoVisitanteId).subscribe({
          next: (jugadoresVisitante) => {
            this.jugadores = [...jugadoresLocal, ...jugadoresVisitante];
            this.cdr.detectChanges();
          }
        });
      }
    });
  }

  cargarEventos() {
    this.arbitroService.obtenerEventos(this.partidoId).subscribe({
      next: (data) => {
        this.eventos = data;
        this.cdr.detectChanges();
      }
    });
  }

  agregarEvento() {
    if (this.nuevoEvento.jugadorId === 0 || this.nuevoEvento.minuto <= 0) return;
    this.cargandoEvento = true;
    this.arbitroService.añadirEvento(this.partidoId, this.nuevoEvento).subscribe({
      next: () => {
        this.nuevoEvento = { jugadorId: 0, minuto: 0, tipoEvento: 'Gol' };
        this.jugadorSeleccionadoNombre = 'Selecciona jugador';
        this.cargandoEvento = false;
        this.cdr.detectChanges();
        this.cargarEventos();
      },
      error: () => {
        this.cargandoEvento = false;
        this.cdr.detectChanges();
      }
    });
  }

  eliminarEvento(eventoId: number) {
    this.arbitroService.eliminarEvento(this.partidoId, eventoId).subscribe({
      next: () => { this.cargarEventos(); }
    });
  }

  cerrarActa() {
    this.abrirConfirm(
      'Cerrar Acta',
      '¿Seguro que quieres cerrar el acta? Esta acción no se puede deshacer.',
      'Cerrar Acta',
      () => {
        this.arbitroService.cerrarActa(this.partidoId).subscribe({
          next: () => { this.router.navigate(['/arbitro/historial']); }
        });
      }
    );
  }

  getIconoEvento(tipo: string): string {
    switch (tipo) {
      case 'Gol': return 'fas fa-futbol text-green-500';
      case 'TarjetaAmarilla': return 'fas fa-square text-yellow-500';
      case 'TarjetaRoja': return 'fas fa-square text-red-500';
      default: return 'fas fa-question';
    }
  }
}
