import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { ArbitroService } from '../../../../services/arbitro.service';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-arbitro-acta',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './arbitro-acta.html',
  styleUrl: './arbitro-acta.css'
})
export class AbitroActa implements OnInit {
  formulario = {
    partideId: 0,
    golesLocal: 0,
    golesVisitante: 0,
    tarjetasAmarillas: '',
    tarjetasRojas: '',
    observaciones: ''
  };

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
    }
  }

  guardarActa() {
    if (this.formulario.partideId > 0) {
      this.cargando = true;
      this.arbitroService.crearActa(this.formulario).subscribe({
        next: () => {
          this.cargando = false;
          alert('Acta guardada correctamente');
          this.resetFormulario();
        },
        error: (error) => {
          this.cargando = false;
          console.error('Error guardando acta:', error);
        }
      });
    }
  }

  resetFormulario() {
    this.formulario = {
      partideId: 0,
      golesLocal: 0,
      golesVisitante: 0,
      tarjetasAmarillas: '',
      tarjetasRojas: '',
      observaciones: ''
    };
  }
}

