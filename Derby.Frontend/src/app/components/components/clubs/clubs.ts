import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../navbar/navbar.component';
import { EquipoService } from '../../../services/equipo.service';
import { AdminService } from '../../../services/admin.service';
import { Equipo } from '../../../models/equipo.model';

@Component({
  selector: 'app-clubs',
  standalone: true,
  imports: [CommonModule, FormsModule, NavbarComponent],
  templateUrl: './clubs.html',
  styleUrl: './clubs.css',
})
export class Clubs implements OnInit {
  textoBusqueda: string = '';
  filtroLiga: string = '';
  clubes: Equipo[] = [];
  cargando = false;
  modalAbierto = false;
  equipoSeleccionado: Equipo | null = null;
  jugadores: any[] = [];
  cargandoJugadores = false;

  constructor(private equipoService: EquipoService, private adminService: AdminService) {}

  ngOnInit(): void {
    this.cargando = true;
    this.equipoService.getEquipos().subscribe({
      next: (data) => {
        this.clubes = data;
        this.cargando = false;
      },
      error: () => {
        this.cargando = false;
      }
    });
  }

  get ligas(): string[] {
    return [...new Set(this.clubes.map(c => c.ligaNombre).filter(l => l && l !== 'Sin Liga'))] as string[];
  }

  get clubesFiltrados() {
    return this.clubes.filter(club => {
      const coincideNombre = club.nombre.toLowerCase().includes(this.textoBusqueda.toLowerCase());
      const coincideLiga = !this.filtroLiga || club.ligaNombre === this.filtroLiga;
      return coincideNombre && coincideLiga;
    }).sort((a, b) => a.nombre.localeCompare(b.nombre));
  }

  verPlantilla(club: Equipo): void {
    this.equipoSeleccionado = club;
    this.jugadores = [];
    this.modalAbierto = true;
    this.cargandoJugadores = true;
    this.adminService.obtenerJugadores(club.id).subscribe({
      next: (data) => {
        this.jugadores = data;
        this.cargandoJugadores = false;
      },
      error: () => {
        this.cargandoJugadores = false;
      }
    });
  }

  cerrarModal(): void {
    this.modalAbierto = false;
    this.equipoSeleccionado = null;
    this.jugadores = [];
  }
}
