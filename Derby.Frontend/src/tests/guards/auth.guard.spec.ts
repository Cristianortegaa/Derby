import { TestBed } from '@angular/core/testing';
import { AuthGuard } from '../../app/guards/auth.guard';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { vi } from 'vitest';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        AuthGuard,
        { provide: Router, useValue: router }
      ]
    });
    guard = TestBed.inject(AuthGuard);
  });

  afterEach(() => {
    localStorage.removeItem('usuarioActual');
  });

  it('debería crearse', () => expect(guard).toBeTruthy());

  it('debería permitir acceso si está autenticado', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ email: 'test@test.com', rol: 'Administrador' }));
    guard['authService']['cargarUsuarioGuardado']();
    const result = guard.canActivate();
    expect(result).toBe(true);
  });

  it('debería redirigir a /login si no está autenticado', () => {
    localStorage.removeItem('usuarioActual');
    guard['authService']['usuarioActual'].next(null);
    const result = guard.canActivate();
    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });
});
