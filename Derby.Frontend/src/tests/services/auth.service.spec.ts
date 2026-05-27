import { TestBed } from '@angular/core/testing';
import { AuthService } from '../../app/services/auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;
  const API = 'http://localhost:5101/api/usuarios';

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [AuthService]
    });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
    localStorage.removeItem('usuarioActual');
  });

  afterEach(() => {
    http.verify();
    localStorage.removeItem('usuarioActual');
  });

  it('debería crearse', () => expect(service).toBeTruthy());

  it('login() debería hacer POST a /api/usuarios/login', () => {
    service.login({ email: 'test@test.com', contrasena: 'pass' }).subscribe();
    const req = http.expectOne(`${API}/login`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual({ email: 'test@test.com', contrasena: 'pass' });
    req.flush({ id: 1, email: 'test@test.com', rol: 'Aficionado', token: 'fake-token' });
  });

  it('logout() debería limpiar localStorage', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ email: 'test@test.com' }));
    service.logout();
    expect(localStorage.getItem('usuarioActual')).toBeNull();
  });

  it('estaAutenticado() debería retornar false si no hay usuario', () => {
    service['usuarioActual'].next(null);
    expect(service.estaAutenticado()).toBe(false);
  });

  it('estaAutenticado() debería retornar true tras login', () => {
    service.login({ email: 'test@test.com', contrasena: 'pass' }).subscribe();
    const req = http.expectOne(`${API}/login`);
    req.flush({ id: 1, email: 'test@test.com', rol: 'Aficionado', token: 'fake-token' });
    expect(service.estaAutenticado()).toBe(true);
  });

  it('obtenerRol() debería retornar el rol del usuario', () => {
    service['usuarioActual'].next({ id: 1, email: 'a@a.com', rol: 'Administrador', token: 'x' });
    expect(service.obtenerRol()).toBe('Administrador');
  });

  it('obtenerRol() debería retornar null si no hay usuario', () => {
    service['usuarioActual'].next(null);
    expect(service.obtenerRol()).toBeNull();
  });
});
