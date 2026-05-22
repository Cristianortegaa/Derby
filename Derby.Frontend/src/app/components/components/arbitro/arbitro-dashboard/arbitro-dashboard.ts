﻿import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-arbitro-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent, RouterLink],
  templateUrl: './arbitro-dashboard.html',
  styleUrl: './arbitro-dashboard.css'
})
export class AbitroDashboard implements OnInit {

  usuario: any = null;

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    this.usuario = this.authService.obtenerUsuarioActual();
    if (!this.usuario || this.usuario.rol !== 'Arbitro') {
      this.router.navigate(['/']);
    }
  }
}



