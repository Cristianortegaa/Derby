import { Component, OnInit } from '@angular/core';
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

  constructor(
    private arbitroService: ArbitroService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    const usuario = this.authService.obtenerUsuarioActual();
    if (usuario) {
      this.arbitroId = usuario.id;
      this.cargarHistorial();
    }
  }

  cargarHistorial() {
    this.cargando = true;
    this.arbitroService.obtenerHistorialPartidos(this.arbitroId).subscribe({
      next: (data) => {
        this.partidos = data;
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error cargando historial:', error);
        this.cargando = false;
      }
    });
  }
}

