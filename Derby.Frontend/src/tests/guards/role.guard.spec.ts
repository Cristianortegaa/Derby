import { TestBed } from '@angular/core/testing';
import { RoleGuard } from '../../app/guards/role.guard';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { vi } from 'vitest';

describe('RoleGuard', () => {
  let guard: RoleGuard;
  let router: { navigate: ReturnType<typeof vi.fn> };

  beforeEach(() => {
    router = { navigate: vi.fn() };

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        RoleGuard,
        { provide: Router, useValue: router }
      ]
    });
    guard = TestBed.inject(RoleGuard);
  });

  afterEach(() => {
    localStorage.removeItem('usuarioActual');
    guard['authService']['usuarioActual'].next(null);
  });

  const setUsuario = (rol: string) => {
    localStorage.setItem('usuarioActual', JSON.stringify({ rol }));
    guard['authService']['cargarUsuarioGuardado']();
  };

  it('debería crearse', () => expect(guard).toBeTruthy());

  it('debería permitir si el rol Administrador coincide con Admin', () => {
    setUsuario('Administrador');
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any);
    expect(result).toBe(true);
  });

  it('debería denegar si el rol no coincide', () => {
    setUsuario('Aficionado');
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any);
    expect(result).toBe(false);
  });

  it('debería permitir Arbitro a rutas de Arbitro', () => {
    setUsuario('Arbitro');
    const result = guard.canActivate({ data: { rol: 'Arbitro' } } as any);
    expect(result).toBe(true);
  });

  it('debería permitir Aficionado a rutas de Aficionado', () => {
    setUsuario('Aficionado');
    const result = guard.canActivate({ data: { rol: 'Aficionado' } } as any);
    expect(result).toBe(true);
  });

  it('debería redirigir al inicio si falla la autorización', () => {
    setUsuario('Aficionado');
    guard.canActivate({ data: { rol: 'Admin' } } as any);
    expect(router.navigate).toHaveBeenCalledWith(['/']);
  });

  it('debería denegar y redirigir a login si no hay usuario', () => {
    guard['authService']['usuarioActual'].next(null);
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any);
    expect(result).toBe(false);
    expect(router.navigate).toHaveBeenCalledWith(['/login']);
  });

  it('debería denegar si el usuario no tiene rol', () => {
    setUsuario('');
    const result = guard.canActivate({ data: { rol: 'Admin' } } as any);
    expect(result).toBe(false);
  });
});
