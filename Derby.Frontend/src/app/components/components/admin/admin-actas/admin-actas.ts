import {Component, OnInit} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule} from '@angular/forms';
import {RouterLink} from '@angular/router';
import {NavbarComponent} from '../../../navbar/navbar.component';
import {AdminService} from '../../../../services/admin.service';

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
  editando: any = null;
  formulario = {golesLocal: 0, golesVisitante: 0};
  eventos: any[] = [];

  notificacion = {mostrar: false, tipo: 'exito' as 'exito' | 'error', mensaje: ''};

  constructor(private adminService: AdminService) {
  }

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
      error: () => {
        this.cargando = false;
      }
    });
  }

  abrirEditar(acta: any) {
    this.editando = acta;
    this.formulario = {golesLocal: acta.golesLocal ?? 0, golesVisitante: acta.golesVisitante ?? 0};
    this.eventos = [];
    this.adminService.obtenerEventosPartido(acta.id).subscribe({
      next: (data) => this.eventos = data,
      error: () => this.eventos = []
    });
  }

  cerrarEditar() {
    this.editando = null;
    this.eventos = [];
  }

  guardar() {
    this.adminService.actualizarActa(this.editando.id, this.formulario).subscribe({
      next: () => {
        this.mostrarNotificacion('exito', 'Acta actualizada correctamente');
        this.cerrarEditar();
        this.cargarActas();
      },
      error: () => this.mostrarNotificacion('error', 'Error al actualizar el acta')
    });
  }

  mostrarNotificacion(tipo: 'exito' | 'error', mensaje: string) {
    this.notificacion = {mostrar: true, tipo, mensaje};
    setTimeout(() => this.notificacion.mostrar = false, 3000);
  }
}

