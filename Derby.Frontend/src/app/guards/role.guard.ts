﻿import { Injectable } from '@angular/core';
import { CanActivate, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class RoleGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot): boolean {
    const rolRequerido = route.data['rol'];
    const usuarioActual = this.authService.obtenerUsuarioActual();

    if (!usuarioActual) {
      this.router.navigate(['/login']);
      return false;
    }

    // Normalizar roles para comparación
    const rolUsuario = usuarioActual.rol;
    const rolRequeridoNormalizado = rolRequerido === 'Admin' ? 'Administrador' : rolRequerido;

    if (rolUsuario === rolRequeridoNormalizado) {
      return true;
    }

    this.router.navigate(['/']);
    return false;
  }
}

