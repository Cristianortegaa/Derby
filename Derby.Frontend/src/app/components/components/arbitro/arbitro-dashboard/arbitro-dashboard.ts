﻿import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AuthService } from '../../../../services/auth.service';

@Component({
  selector: 'app-arbitro-dashboard',
  standalone: true,
  imports: [CommonModule, NavbarComponent],
  templateUrl: './arbitro-dashboard.html',
  styleUrl: './arbitro-dashboard.css'
})
export class AbitroDashboard implements OnInit {

  constructor(private authService: AuthService, private router: Router) {}

  ngOnInit() {
    const usuario = this.authService.obtenerUsuarioActual();
    if (!usuario || usuario.rol !== 'Arbitro') {
      this.router.navigate(['/']);
    }
  }
}



