import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AdminService } from '../../app/services/admin.service';

describe('AdminService', () => {
  let service: AdminService;
  let http: HttpTestingController;
  const API = 'http://localhost:5101/api';

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule], providers: [AdminService] });
    service = TestBed.inject(AdminService);
    http    = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('debería crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  // =========================================================================
  // Usuarios
  // =========================================================================

  it('obtenerUsuarios() debería hacer GET a /api/usuarios', () => {
    service.obtenerUsuarios().subscribe();
    http.expectOne(`${API}/usuarios`).flush([]);
  });

  it('crearUsuario() debería hacer POST a /api/usuarios/registro', () => {
    service.crearUsuario({ email: 'x@x.com' }).subscribe();
    const req = http.expectOne(`${API}/usuarios/registro`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('actualizarUsuario(5) debería hacer PUT a /api/usuarios/5', () => {
    service.actualizarUsuario(5, { email: 'new@x.com' }).subscribe();
    const req = http.expectOne(`${API}/usuarios/5`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('eliminarUsuario(3) debería hacer DELETE a /api/usuarios/3', () => {
    service.eliminarUsuario(3).subscribe();
    const req = http.expectOne(`${API}/usuarios/3`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  // =========================================================================
  // Competiciones
  // =========================================================================

  it('obtenerCompeticiones() debería hacer GET a /api/admin/competiciones', () => {
    service.obtenerCompeticiones().subscribe();
    http.expectOne(`${API}/admin/competiciones`).flush([]);
  });

  it('crearCompeticion() debería hacer POST a /api/admin/competiciones', () => {
    service.crearCompeticion({ nombre: 'X' }).subscribe();
    const req = http.expectOne(`${API}/admin/competiciones`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('actualizarCompeticion(1) debería hacer PUT a /api/admin/competiciones/1', () => {
    service.actualizarCompeticion(1, { nombre: 'Y' }).subscribe();
    const req = http.expectOne(`${API}/admin/competiciones/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('eliminarCompeticion(1) debería hacer DELETE a /api/admin/competiciones/1', () => {
    service.eliminarCompeticion(1).subscribe();
    const req = http.expectOne(`${API}/admin/competiciones/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  // =========================================================================
  // Ligas
  // =========================================================================

  it('obtenerLigas() debería hacer GET a /api/admin/ligas', () => {
    service.obtenerLigas().subscribe();
    http.expectOne(`${API}/admin/ligas`).flush([]);
  });

  it('crearLiga() debería hacer POST a /api/admin/ligas', () => {
    service.crearLiga({ nombre: 'Primera DAW' }).subscribe();
    const req = http.expectOne(`${API}/admin/ligas`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('actualizarLiga(2) debería hacer PUT a /api/admin/ligas/2', () => {
    service.actualizarLiga(2, { nombre: 'Segunda DAW' }).subscribe();
    const req = http.expectOne(`${API}/admin/ligas/2`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('eliminarLiga(2) debería hacer DELETE a /api/admin/ligas/2', () => {
    service.eliminarLiga(2).subscribe();
    const req = http.expectOne(`${API}/admin/ligas/2`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  // =========================================================================
  // Equipos
  // =========================================================================

  it('obtenerEquipos() debería hacer GET a /api/admin/equipos', () => {
    service.obtenerEquipos().subscribe();
    http.expectOne(`${API}/admin/equipos`).flush([]);
  });

  it('crearEquipo() debería hacer POST a /api/admin/equipos', () => {
    service.crearEquipo({ nombre: 'FC Derby Norte' }).subscribe();
    const req = http.expectOne(`${API}/admin/equipos`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('actualizarEquipo(3) debería hacer PUT a /api/admin/equipos/3', () => {
    service.actualizarEquipo(3, { nombre: 'Actualizado' }).subscribe();
    const req = http.expectOne(`${API}/admin/equipos/3`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('eliminarEquipo(3) debería hacer DELETE a /api/admin/equipos/3', () => {
    service.eliminarEquipo(3).subscribe();
    const req = http.expectOne(`${API}/admin/equipos/3`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('obtenerEquiposSinLiga() debería hacer GET a /api/admin/equipos/sin-liga', () => {
    service.obtenerEquiposSinLiga().subscribe();
    http.expectOne(`${API}/admin/equipos/sin-liga`).flush([]);
  });

  // =========================================================================
  // Árbitros
  // =========================================================================

  it('obtenerArbitros() debería hacer GET a /api/admin/arbitros', () => {
    service.obtenerArbitros().subscribe();
    http.expectOne(`${API}/admin/arbitros`).flush([]);
  });

  it('crearArbitro() debería hacer POST a /api/admin/arbitros', () => {
    service.crearArbitro({ nombre: 'Jorge', apellidos: 'Blanco' }).subscribe();
    const req = http.expectOne(`${API}/admin/arbitros`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('actualizarArbitro(1) debería hacer PUT a /api/admin/arbitros/1', () => {
    service.actualizarArbitro(1, { nombre: 'Nuevo' }).subscribe();
    const req = http.expectOne(`${API}/admin/arbitros/1`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('eliminarArbitro(1) debería hacer DELETE a /api/admin/arbitros/1', () => {
    service.eliminarArbitro(1).subscribe();
    const req = http.expectOne(`${API}/admin/arbitros/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  // =========================================================================
  // Partidos
  // =========================================================================

  it('obtenerPartidos() debería hacer GET a /api/admin/partidos', () => {
    service.obtenerPartidos().subscribe();
    http.expectOne(`${API}/admin/partidos`).flush([]);
  });

  it('crearPartido() debería hacer POST a /api/admin/partidos con el body correcto', () => {
    const payload = { ligaId: 1, equipoLocalId: 2, equipoVisitanteId: 3, jornada: 1 };
    service.crearPartido(payload).subscribe();
    const req = http.expectOne(`${API}/admin/partidos`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush({ id: 10, ...payload });
  });

  it('actualizarPartido(10) debería hacer PUT a /api/admin/partidos/10', () => {
    service.actualizarPartido(10, { estado: 'Finalizado' }).subscribe();
    const req = http.expectOne(`${API}/admin/partidos/10`);
    expect(req.request.method).toBe('PUT');
    req.flush({});
  });

  it('eliminarPartido(10) debería hacer DELETE a /api/admin/partidos/10', () => {
    service.eliminarPartido(10).subscribe();
    const req = http.expectOne(`${API}/admin/partidos/10`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  // =========================================================================
  // Liga-Equipos
  // =========================================================================

  it('obtenerEquiposLiga(2) debería hacer GET a /api/admin/ligas/2/equipos', () => {
    service.obtenerEquiposLiga(2).subscribe();
    http.expectOne(`${API}/admin/ligas/2/equipos`).flush([]);
  });

  it('añadirEquipoLiga(2,5) debería hacer POST a /api/admin/ligas/2/equipos', () => {
    service.añadirEquipoLiga(2, 5).subscribe();
    const req = http.expectOne(`${API}/admin/ligas/2/equipos`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('quitarEquipoLiga(2,5) debería hacer DELETE a /api/admin/ligas/2/equipos/5', () => {
    service.quitarEquipoLiga(2, 5).subscribe();
    const req = http.expectOne(`${API}/admin/ligas/2/equipos/5`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });

  it('generarCalendario(2) debería hacer POST a /api/admin/ligas/2/generar-calendario', () => {
    service.generarCalendario(2).subscribe();
    const req = http.expectOne(`${API}/admin/ligas/2/generar-calendario`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  // =========================================================================
  // Jugadores
  // =========================================================================

  it('obtenerJugadores(5) debería hacer GET a /api/admin/equipos/5/jugadores', () => {
    service.obtenerJugadores(5).subscribe();
    http.expectOne(`${API}/admin/equipos/5/jugadores`).flush([]);
  });

  it('agregarJugador(5,…) debería hacer POST a /api/admin/equipos/5/jugadores', () => {
    service.agregarJugador(5, { nombre: 'Leo', dorsal: 10 }).subscribe();
    const req = http.expectOne(`${API}/admin/equipos/5/jugadores`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });

  it('eliminarJugador(1) debería hacer DELETE a /api/admin/jugadores/1', () => {
    service.eliminarJugador(1).subscribe();
    const req = http.expectOne(`${API}/admin/jugadores/1`);
    expect(req.request.method).toBe('DELETE');
    req.flush(null);
  });
});

