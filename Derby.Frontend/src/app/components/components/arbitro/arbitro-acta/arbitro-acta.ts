import { Component, OnInit } from '@angular/core';
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

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private arbitroService: ArbitroService,
    private adminService: AdminService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.partidoId = Number(this.route.snapshot.paramMap.get('partidoId'));
    const usuario = this.authService.obtenerUsuarioActual();
    if (usuario) {
      this.cargarPartido(usuario.id);
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
      },
      error: () => { this.cargando = false; }
    });
  }

  cargarJugadores() {
    this.adminService.obtenerJugadores(this.partido.equipoLocalId).subscribe({
      next: (jugadoresLocal) => {
        this.adminService.obtenerJugadores(this.partido.equipoVisitanteId).subscribe({
          next: (jugadoresVisitante) => {
            this.jugadores = [...jugadoresLocal, ...jugadoresVisitante];
          }
        });
      }
    });
  }

  cargarEventos() {
    this.arbitroService.obtenerEventos(this.partidoId).subscribe({
      next: (data) => { this.eventos = data; }
    });
  }

  añadirEvento() {
    if (this.nuevoEvento.jugadorId === 0 || this.nuevoEvento.minuto <= 0) return;
    this.cargandoEvento = true;
    this.arbitroService.añadirEvento(this.partidoId, this.nuevoEvento).subscribe({
      next: () => {
        this.cargarEventos();
        this.nuevoEvento = { jugadorId: 0, minuto: 0, tipoEvento: 'Gol' };
        this.cargandoEvento = false;
      },
      error: () => { this.cargandoEvento = false; }
    });
  }

  eliminarEvento(eventoId: number) {
    this.arbitroService.eliminarEvento(this.partidoId, eventoId).subscribe({
      next: () => { this.cargarEventos(); }
    });
  }

  cerrarActa() {
    if (!confirm('¿Seguro que quieres cerrar el acta? Esta acción no se puede deshacer.')) return;
    this.arbitroService.cerrarActa(this.partidoId).subscribe({
      next: () => { this.router.navigate(['/arbitro/historial']); }
    });
  }

  getIconoEvento(tipo: string): string {
    switch (tipo) {
      case 'Gol': return 'fas fa-futbol text-green-500';
      case 'Tarjeta Amarilla': return 'fas fa-square text-yellow-500';
      case 'Tarjeta Roja': return 'fas fa-square text-red-500';
      default: return 'fas fa-question';
    }
  }
}
