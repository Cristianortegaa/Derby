import { TestBed } from '@angular/core/testing';
import { AuthService } from '../../app/services/auth.service';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

describe('AuthService', () => {
  let service: AuthService;
  let http: HttpTestingController;
  const API = 'http://localhost:5101/api';

  beforeEach(() => {
    TestBed.configureTestingModule({ imports: [HttpClientTestingModule], providers: [AuthService] });
    service = TestBed.inject(AuthService);
    http = TestBed.inject(HttpTestingController);
  });

  afterEach(() => http.verify());

  it('debería crearse', () => expect(service).toBeTruthy());
  it('login() debería hacer POST a /api/auth/login', () => {
    service.login('test@test.com', 'pass').subscribe();
    const req = http.expectOne(`${API}/auth/login`);
    expect(req.request.method).toBe('POST');
    req.flush({});
  });
  it('logout() debería limpiar localStorage', () => {
    localStorage.setItem('usuarioActual', 'test');
    service.logout();
    expect(localStorage.getItem('usuarioActual')).toBeNull();
  });
  it('estaAutenticado() debería retornar true si hay usuario', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ email: 'test@test.com' }));
    expect(service.estaAutenticado()).toBe(true);
    localStorage.removeItem('usuarioActual');
  });
});

