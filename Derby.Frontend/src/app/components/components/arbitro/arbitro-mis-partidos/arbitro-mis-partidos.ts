import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { ArbitroService } from '../../../../services/arbitro.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-arbitro-mis-partidos',
  standalone: true,
  imports: [CommonModule, RouterLink, NavbarComponent],
  templateUrl: './arbitro-mis-partidos.html',
  styleUrl: './arbitro-mis-partidos.css'
})
export class AbitroMisPartidos implements OnInit {
  partidos: any[] = [];
  cargando = false;
  arbitroId: number = 0;

  constructor(
    private arbitroService: ArbitroService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    const usuario = this.authService.obtenerUsuarioActual();
    if (usuario) {
      this.arbitroId = usuario.id;
      this.cargarPartidos();
    }
  }

  cargarPartidos() {
    this.cargando = true;
    this.arbitroService.obtenerMisPartidos(this.arbitroId).subscribe({
      next: (data) => {
        this.partidos = data;
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error cargando partidos:', error);
        this.cargando = false;
      }
    });
  }

  getEstiloEstado(estado: string): string {
    switch (estado) {
      case 'Programado':
        return 'bg-blue-500/20 text-blue-400';
      case 'En juego':
        return 'bg-amber-500/20 text-amber-400';
      case 'Finalizado':
        return 'bg-green-500/20 text-green-400';
      default:
        return 'bg-gray-500/20 text-gray-400';
    }
  }
}

