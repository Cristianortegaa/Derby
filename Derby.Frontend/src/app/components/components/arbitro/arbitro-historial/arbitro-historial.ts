import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { ArbitroService } from '../../../../services/arbitro.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-arbitro-historial',
  standalone: true,
  imports: [CommonModule, RouterLink, NavbarComponent],
  templateUrl: './arbitro-historial.html',
  styleUrl: './arbitro-historial.css'
})
export class AbitroHistorial implements OnInit {
  partidos: any[] = [];
  cargando = false;
  arbitroId: number = 0;

  modalDetalle: any = { mostrar: false, partido: null };
  eventosDetalle: any[] = [];
  cargandoEventos = false;

  constructor(
    private arbitroService: ArbitroService,
    private authService: AuthService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    const usuario = this.authService.obtenerUsuarioActual();
    if (usuario) {
      this.arbitroId = usuario.arbitroId ?? 0;
      this.cargarHistorial();
    }
  }

  cargarHistorial() {
    this.cargando = true;
    this.arbitroService.obtenerHistorialPartidos(this.arbitroId).subscribe({
      next: (data) => {
        data.sort((a: any, b: any) =>
          new Date(b.fechaHora).getTime() - new Date(a.fechaHora).getTime()
        );
        this.partidos = data;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error cargando historial:', error);
        this.cargando = false;
      }
    });
  }

  abrirDetalle(partido: any) {
    this.modalDetalle = { mostrar: true, partido };
    this.eventosDetalle = [];
    this.cargandoEventos = true;
    this.arbitroService.obtenerEventos(partido.id).subscribe({
      next: (data) => {
        this.eventosDetalle = data;
        this.cargandoEventos = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cargandoEventos = false;
      }
    });
  }

  cerrarDetalle() {
    this.modalDetalle.mostrar = false;
  }

  getIconoEvento(tipo: string): string {
    switch (tipo) {
      case 'Gol': return 'fas fa-futbol text-green-400';
      case 'TarjetaAmarilla': return 'fas fa-square text-yellow-400';
      case 'TarjetaRoja': return 'fas fa-square text-red-500';
      default: return 'fas fa-circle text-gray-400';
    }
  }
}

