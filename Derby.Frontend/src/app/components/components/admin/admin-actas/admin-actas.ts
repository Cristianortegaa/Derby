import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-actas',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-actas.html',
  styleUrl: './admin-actas.css'
})
export class AdminActas implements OnInit {
  actas: any[] = [];
  cargando = false;

  modalConfirm = { mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Eliminar', onConfirm: () => {} };

  abrirConfirm(titulo: string, mensaje: string, accion: () => void, textoConfirmar = 'Eliminar') {
    this.modalConfirm = { mostrar: true, titulo, mensaje, textoConfirmar, onConfirm: accion };
  }
  cerrarConfirm() { this.modalConfirm.mostrar = false; }
  confirmar() { this.modalConfirm.onConfirm(); this.cerrarConfirm(); }

  constructor(private adminService: AdminService) {}

  ngOnInit() {
    this.cargarActas();
  }

  cargarActas() {
    this.cargando = true;
    this.adminService.obtenerActas().subscribe({
      next: (data) => {
        this.actas = data;
        this.cargando = false;
      },
      error: (error) => {
        console.error('Error cargando actas:', error);
        this.cargando = false;
      }
    });
  }

  eliminarActa(id: number) {
    this.abrirConfirm('Eliminar acta', '¿Estás seguro de eliminar esta acta?', () => {
      this.adminService.eliminarActa(id).subscribe({
        next: () => this.cargarActas(),
        error: (error) => console.error('Error eliminando:', error)
      });
    });
  }
}

