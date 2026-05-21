import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';
import { CompeticionesService } from '../../../../services/competiciones.service';

@Component({
  selector: 'app-admin-goleadores',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-goleadores.html',
  styleUrls: ['./admin-goleadores.css']
})
export class AdminGoleadoresComponent implements OnInit {
  goleadores: any[] = [];
  goleadoresFiltrados: any[] = [];
  ligas: any[] = [];
  equipos: any[] = [];
  cargando = false;
  busquedaBuscador = '';
  competicionIdFijado: number | null = null;
  ligaIdFijado: number | null = null;

  constructor(
    private adminService: AdminService,
    private competicionesService: CompeticionesService,
    private cdr: ChangeDetectorRef,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    const cid = this.route.snapshot.queryParams['competicionId'];
    if (cid) this.competicionIdFijado = Number(cid);
    const lid = this.route.snapshot.queryParams['ligaId'];
    if (lid) this.ligaIdFijado = Number(lid);
    this.cargarDatos();
  }

  async cargarDatos(): Promise<void> {
    this.cargando = true;
    try {
      const [todasLigas, equipos] = await Promise.all([
        this.adminService.obtenerLigas().toPromise(),
        this.adminService.obtenerEquipos().toPromise()
      ]);

      const ligas = todasLigas || [];
      this.ligas = this.ligaIdFijado
        ? ligas.filter((l: any) => l.id === this.ligaIdFijado)
        : this.competicionIdFijado
          ? ligas.filter((l: any) => l.competicionId === this.competicionIdFijado)
          : ligas;
      this.equipos = equipos || [];
      if (this.ligaIdFijado) {
        const goleadores = await this.competicionesService.obtenerGoleadoresPorLiga(this.ligaIdFijado).toPromise();
        this.goleadores = goleadores || [];
      } else if (this.competicionIdFijado) {
        const goleadores = await this.competicionesService.obtenerGoleadores(this.competicionIdFijado).toPromise();
        this.goleadores = goleadores || [];
      } else {
        this.goleadores = [];
      }
      this.aplicarFiltro();
    } catch (error) {
      console.error(error);
    } finally {
      this.cargando = false;
      this.cdr.detectChanges();
    }
  }

  aplicarFiltro(): void {
    this.goleadoresFiltrados = this.goleadores.filter(g =>
      g.jugador?.nombre?.toLowerCase().includes(this.busquedaBuscador.toLowerCase()) ||
      g.equipo?.nombre?.toLowerCase().includes(this.busquedaBuscador.toLowerCase())
    );
  }
}

