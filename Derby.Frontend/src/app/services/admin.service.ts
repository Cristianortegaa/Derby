﻿import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  private apiUrl = 'http://localhost:5101/api';

  constructor(private http: HttpClient) {}

  // Usuarios
  obtenerUsuarios(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/usuarios`);
  }

  obtenerUsuario(id: number): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/usuarios/${id}`);
  }

  crearUsuario(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/usuarios/registro`, datos);
  }

  actualizarUsuario(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/usuarios/${id}`, datos);
  }

  eliminarUsuario(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/usuarios/${id}`);
  }

  // Competiciones
  obtenerCompeticiones(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/competiciones`);
  }

  crearCompeticion(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/competiciones`, datos);
  }

  actualizarCompeticion(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/competiciones/${id}`, datos);
  }

  eliminarCompeticion(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/competiciones/${id}`);
  }

  // Equipos
  obtenerEquipos(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/equipos`);
  }

  crearEquipo(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/equipos`, datos);
  }

  actualizarEquipo(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/equipos/${id}`, datos);
  }

  eliminarEquipo(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/equipos/${id}`);
  }

  // Árbitros
  obtenerArbitros(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/arbitros`);
  }

  crearArbitro(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/arbitros`, datos);
  }

  actualizarArbitro(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/arbitros/${id}`, datos);
  }

  eliminarArbitro(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/arbitros/${id}`);
  }

  // Actas
  obtenerActas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/actas`);
  }

  crearActa(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/actas`, datos);
  }

  actualizarActa(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/actas/${id}`, datos);
  }

  eliminarActa(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/actas/${id}`);
  }

  obtenerLigas(): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/admin/ligas`);
  }

  crearLiga(datos: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/admin/ligas`, datos);
  }

  actualizarLiga(id: number, datos: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/admin/ligas/${id}`, datos);
  }

  eliminarLiga(id: number): Observable<any> {
    return this.http.delete(`${this.apiUrl}/admin/ligas/${id}`);
  }
}

