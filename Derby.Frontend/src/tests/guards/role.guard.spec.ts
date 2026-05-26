import { TestBed } from '@angular/core/testing';
import { RoleGuard } from '../../app/guards/role.guard';
import { Router } from '@angular/router';

describe('RoleGuard', () => {
  let guard: RoleGuard;
  let router: Router;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [RoleGuard, { provide: Router, useValue: { navigate: jasmine.createSpy('navigate') } }]
    });
    guard = TestBed.inject(RoleGuard);
    router = TestBed.inject(Router);
  });

  it('debería crearse', () => expect(guard).toBeTruthy());

  it('debería permitir si el rol coincide', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol: 'Admin' }));
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any, { url: '/admin' } as any);
    expect(result).toBe(true);
    localStorage.removeItem('usuarioActual');
  });

  it('debería denegar si el rol no coincide', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol: 'Aficionado' }));
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any, { url: '/admin' } as any);
    expect(result).toBe(false);
    localStorage.removeItem('usuarioActual');
  });

  it('debería permitir Arbitro a rutas de Arbitro', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol: 'Arbitro' }));
    const result = guard.canActivate({ data: { rol: 'Arbitro' } } as any, { url: '/arbitro' } as any);
    expect(result).toBe(true);
    localStorage.removeItem('usuarioActual');
  });

  it('debería permitir Aficionado a rutas de Aficionado', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol: 'Aficionado' }));
    const result = guard.canActivate({ data: { rol: 'Aficionado' } } as any, { url: '/inicio' } as any);
    expect(result).toBe(true);
    localStorage.removeItem('usuarioActual');
  });

  it('debería redirigir si falla la autorización', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol: 'Aficionado' }));
    guard.canActivate({ data: { rol: 'Admin' } } as any, { url: '/admin' } as any);
    expect(router.navigate).toHaveBeenCalledWith(['/']);
    localStorage.removeItem('usuarioActual');
  });

  it('debería denegar si no hay usuario', () => {
    localStorage.removeItem('usuarioActual');
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any, { url: '/admin' } as any);
    expect(result).toBe(false);
  });

  it('debería denegar si el usuario no tiene rol', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ email: 'test@test.com' }));
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any, { url: '/admin' } as any);
    expect(result).toBe(false);
    localStorage.removeItem('usuarioActual');
  });

  it('debería permitir múltiples roles', () => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol: 'Arbitro' }));
    const result = guard.canActivate({ data: { rol: ['Arbitro', 'Admin'] } } as any, { url: '/panel' } as any);
    expect(result).toBe(true);
    localStorage.removeItem('usuarioActual');
  });
});

