﻿﻿import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent, RouterLink],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboard implements OnInit {

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    const usuario = this.authService.obtenerUsuarioActual();
    if (!usuario || usuario.rol !== 'Administrador') {
      this.router.navigate(['/']);
    }
  }
}



