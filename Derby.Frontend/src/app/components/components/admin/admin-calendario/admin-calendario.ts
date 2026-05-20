import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink, ActivatedRoute } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';
import { CompeticionesService, JornadaResponseDto } from '../../../../services/competiciones.service';

@Component({
  selector: 'app-admin-calendario',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-calendario.html',
  styleUrls: ['./admin-calendario.css']
})
export class AdminCalendarioComponent implements OnInit {
  ligas: any[] = [];
  jornadas: JornadaResponseDto[] = [];
  ligaSeleccionada: number = 0;
  cargando = false;
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
    this.cargarLigas();
  }

  async cargarLigas(): Promise<void> {
    try {
      const todasLigas = await this.adminService.obtenerLigas().toPromise();
      const ligas = todasLigas || [];
      this.ligas = this.ligaIdFijado
        ? ligas.filter((l: any) => l.id === this.ligaIdFijado)
        : this.competicionIdFijado
          ? ligas.filter((l: any) => l.competicionId === this.competicionIdFijado)
          : ligas;
      if (this.ligas.length > 0) {
        this.ligaSeleccionada = this.ligas[0].id;
        this.cargarCalendario();
      }
    } catch (error) {
      console.error(error);
    } finally {
      this.cdr.detectChanges();
    }
  }

  async cargarCalendario(): Promise<void> {
    if (!this.ligaSeleccionada) return;
    this.cargando = true;
    try {
      this.jornadas = await this.competicionesService.obtenerJornadasPorLiga(this.ligaSeleccionada).toPromise() || [];
    } catch (error) {
      console.error(error);
      this.jornadas = [];
    } finally {
      this.cargando = false;
      this.cdr.detectChanges();
    }
  }

  cambiarLiga(): void {
    this.cargarCalendario();
  }
}
