import {Component, OnInit, ChangeDetectorRef} from '@angular/core';
import {CommonModule} from '@angular/common';
import {RouterLink, ActivatedRoute} from '@angular/router';
import {NavbarComponent} from '../../../navbar/navbar.component';
import {AdminService} from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-competicion-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, NavbarComponent],
  templateUrl: './admin-competicion-detail.html',
  styleUrl: './admin-competicion-detail.css'
})
export class AdminCompeticionDetail implements OnInit {
  competicion: any = null;
  competicionId: number = 0;
  cargando = true;

  constructor(
    private route: ActivatedRoute,
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {
  }

  ngOnInit() {
    this.competicionId = Number(this.route.snapshot.paramMap.get('id'));
    this.cargarCompeticion();
  }

  cargarCompeticion() {
    this.adminService.obtenerCompeticiones().subscribe({
      next: (data) => {
        this.competicion = data.find((c: any) => c.id === this.competicionId) || null;
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  getBadgeClase(estado: string): string {
    const mapa: Record<string, string> = {
      'Activo': 'badge-activo',
      'Inactivo': 'badge-inactivo',
      'Pausado': 'badge-pausado',
      'Finalizado': 'badge-finalizado'
    };
    return mapa[estado] || 'badge-inactivo';
  }
}
