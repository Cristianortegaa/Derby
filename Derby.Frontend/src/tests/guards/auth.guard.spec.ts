import { TestBed } from '@angular/core/testing';
import { AuthGuard } from '../../app/guards/auth.guard';
import { Router } from '@angular/router';

describe('AuthGuard', () => {
  let guard: AuthGuard;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AuthGuard, { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }]
    });
    guard = TestBed.inject(AuthGuard);
    router = TestBed.inject(Router);
  });

  it('debería crearse', () => expect(guard).toBeTruthy());

  it('debería permitir acceso si está autenticado', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ email: 'test@test.com' }));
    const result = guard.canActivate(
      { data: { rol: 'Admin' } } as any,
      { url: '/admin' } as any
    );
    expect(result).toBe(true);
    localStorage.removeItem('usuarioActual');
  });

  it('debería redirigir a /login si no está autenticado', () => {
    localStorage.removeItem('usuarioActual');
    const result = guard.canActivate(
      { data: { rol: 'Admin' } } as any,
      { url: '/admin' } as any
    );
    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería redirigir si el rol no coincide', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ email: 'test@test.com', rol: 'Aficionado' }));
    const result = guard.canActivate(
      { data: { rol: 'Admin' } } as any,
      { url: '/admin' } as any
    );
    expect(result).toBe(false);
    localStorage.removeItem('usuarioActual');
  });
});

