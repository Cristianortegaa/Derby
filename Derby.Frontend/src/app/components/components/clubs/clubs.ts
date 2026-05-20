import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { NavbarComponent } from '../../navbar/navbar.component';

@Component({
  selector: 'app-clubs',
  standalone: true,
  imports: [FormsModule, NavbarComponent],
  templateUrl: './clubs.html',
  styleUrl: './clubs.css',
})

export class Clubs {
  textoBusqueda: string = '';
  divisionSeleccionada: string = '';

  clubes = [
    { id: 1, nombre: 'Getafe CF', division: '1', sede: 'Polideportivo Getafe' },
    { id: 2, nombre: 'Rayo Vallecano', division: '1', sede: 'Vallecas' },
    { id: 3, nombre: 'Leganés B', division: '2', sede: 'Polideportivo Butarque' },
    { id: 4, nombre: 'Madrid CFF', division: 'fem', sede: 'Antiguo Canódromo' }
  ];

  get clubesFiltrados() {
    return this.clubes.filter(club => {
      let nombreEquipo = club.nombre.toLowerCase();
      let textoBuscado = this.textoBusqueda.toLowerCase();

      let coincideNombre = nombreEquipo.includes(textoBuscado);

      let coincideDivision = (this.divisionSeleccionada === '') || (club.division === this.divisionSeleccionada);

      return coincideNombre && coincideDivision;
    });
  }
}
