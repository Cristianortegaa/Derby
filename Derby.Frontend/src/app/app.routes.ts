import { Routes } from '@angular/router';
import { Home } from './components/components/home/home';
import { Login } from './components/components/login/login';
import { Clubs } from './components/components/clubs/clubs';
import { Competiciones } from './components/components/competiciones/competiciones';
import { AdminDashboard } from './components/components/admin/admin-dashboard/admin-dashboard';
import { AdminUsuarios } from './components/components/admin/admin-usuarios/admin-usuarios';
import { AdminCompeticiones } from './components/components/admin/admin-competiciones/admin-competiciones';
import { AdminLigas } from './components/components/admin/admin-ligas/admin-ligas';
import { AdminLigaDetail } from './components/components/admin/admin-liga-detail/admin-liga-detail';
import { AdminPartidosComponent } from './components/components/admin/admin-partidos/admin-partidos';
import { AdminGoleadoresComponent } from './components/components/admin/admin-goleadores/admin-goleadores';
import { AdminClasificacionComponent } from './components/components/admin/admin-clasificacion/admin-clasificacion';
import { AdminCalendarioComponent } from './components/components/admin/admin-calendario/admin-calendario';
import { AdminCompeticionDetail } from './components/components/admin/admin-competicion-detail/admin-competicion-detail';
import { AdminEquipos } from './components/components/admin/admin-equipos/admin-equipos';
import { AbitroDashboard } from './components/components/arbitro/arbitro-dashboard/arbitro-dashboard';
import { AdminEquipoDetail } from './components/components/admin/admin-equipo-detail/admin-equipo-detail';
import { RoleGuard } from './guards/role.guard';
import { AuthGuard } from './guards/auth.guard';

export const routes: Routes = [
  { path: '', component: Home },
  { path: 'login', component: Login },
  { path: 'intranet', component: Login },
  { path: 'clubes', component: Clubs },
  { path: 'competiciones', component: Competiciones },

  {
    path: 'admin',
    component: AdminDashboard,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/usuarios',
    component: AdminUsuarios,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/competiciones',
    component: AdminCompeticiones,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/competiciones/:id',
    component: AdminCompeticionDetail,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/ligas',
    component: AdminLigas,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/ligas/:id',
    component: AdminLigaDetail,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/partidos',
    component: AdminPartidosComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/goleadores',
    component: AdminGoleadoresComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/clasificacion',
    component: AdminClasificacionComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },
  {
    path: 'admin/calendario',
    component: AdminCalendarioComponent,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },

  {
    path: 'admin/equipos',
    component: AdminEquipos,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },

  {
    path: 'admin/equipos/:id',
    component: AdminEquipoDetail,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Admin' }
  },

  {
    path: 'arbitro',
    component: AbitroDashboard,
    canActivate: [AuthGuard, RoleGuard],
    data: { rol: 'Arbitro' }
  },

  { path: '**', redirectTo: '' }
];
