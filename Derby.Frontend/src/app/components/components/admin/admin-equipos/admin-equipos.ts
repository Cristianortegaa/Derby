import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-equipos',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-equipos.html',
  styleUrl: './admin-equipos.css'
})
export class AdminEquipos implements OnInit {
  equipos: any[] = [];
  cargando = false;
  mostrarForm = false;
  formulario = {
    nombre: '',
    sede: '',
    division: '',
    entrenador: ''
  };

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.cargarEquipos();
  }

  cargarEquipos() {
    this.cargando = true;
    this.adminService.obtenerEquipos().subscribe({
      next: (data) => {
        this.equipos = data;
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error cargando equipos:', error);
        this.cargando = false;
      }
    });
  }

  agregarEquipo() {
    if (this.formulario.nombre && this.formulario.division) {
      this.adminService.crearEquipo(this.formulario).subscribe({
        next: () => {
          this.cargarEquipos();
          this.resetFormulario();
          this.mostrarForm = false;
        },
        error: (error) => console.error('Error creando equipo:', error)
      });
    }
  }

  eliminarEquipo(id: number) {
    if (confirm('¿Estás seguro de eliminar este equipo?')) {
      this.adminService.eliminarEquipo(id).subscribe({
        next: () => this.cargarEquipos(),
        error: (error) => console.error('Error eliminando:', error)
      });
    }
  }

  resetFormulario() {
    this.formulario = {
      nombre: '',
      sede: '',
      division: '',
      entrenador: ''
    };
  }
}

