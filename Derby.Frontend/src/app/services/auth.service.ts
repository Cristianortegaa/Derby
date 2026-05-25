import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { Usuario, LoginRequest, RegistroRequest } from '../models/auth.model';


@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private apiUrl = 'http://localhost:5101/api/usuarios';
  private usuarioActual = new BehaviorSubject<Usuario | null>(null);
  public usuarioActual$ = this.usuarioActual.asObservable();

  constructor(private http: HttpClient) {
    this.cargarUsuarioGuardado();
  }

  registro(datos: RegistroRequest): Observable<Usuario> {
    return this.http.post<Usuario>(`${this.apiUrl}/registro`, datos).pipe(
      tap(usuario => {
        this.guardarUsuario(usuario);
        this.usuarioActual.next(usuario);
      })
    );
  }

  login(datos: LoginRequest): Observable<Usuario> {
    return this.http.post<Usuario>(`${this.apiUrl}/login`, datos).pipe(
      tap(usuario => {
        this.guardarUsuario(usuario);
        this.usuarioActual.next(usuario);
      })
    );
  }

  logout() {
    localStorage.removeItem('usuarioActual');
    this.usuarioActual.next(null);
  }

  private guardarUsuario(usuario: Usuario) {
    localStorage.setItem('usuarioActual', JSON.stringify(usuario));
  }

  private cargarUsuarioGuardado() {
    const usuario = localStorage.getItem('usuarioActual');
    if (usuario) {
      try {
        this.usuarioActual.next(JSON.parse(usuario));
      } catch (e) {
        console.error('Error al cargar usuario guardado', e);
      }
    }
  }

  estaAutenticado(): boolean {
    return this.usuarioActual.value !== null;
  }

  obtenerUsuarioActual(): Usuario | null {
    return this.usuarioActual.value;
  }

  obtenerRol(): string | null {
    return this.usuarioActual.value?.rol || null;
  }

  tieneRol(rol: string): boolean {
    return this.usuarioActual.value?.rol === rol;
  }
}

