import { Component, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { EquipoService } from '../../../services/equipo.service';
import { CommonModule } from '@angular/common';
import { NavbarComponent } from '../../navbar/navbar.component';
import {FooterComponent} from '../../footer/footer.component'

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [RouterLink, CommonModule, NavbarComponent, FooterComponent],
  templateUrl: './home.html',
  styleUrl: './home.css'
})
export class Home implements OnInit {
  equipos: any[] = [];
  loading = false;

  constructor(private equipoService: EquipoService) {}

  ngOnInit() {
    this.loadEquipos();
  }

  loadEquipos() {
    this.loading = true;
    this.equipoService.getEquipos().subscribe({
      next: (data: any) => {
        this.equipos = data;
        this.loading = false;
        console.log('Equipos cargados:', this.equipos);
      },
      error: (error: any) => {
        console.error('Error al cargar equipos:', error);
        this.loading = false;
      }
    });
  }
}
