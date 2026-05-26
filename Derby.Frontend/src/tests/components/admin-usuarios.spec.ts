import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminUsuarios } from '../../app/components/components/admin/admin-usuarios/admin-usuarios';
import { AdminService } from '../../app/services/admin.service';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

describe('AdminUsuarios', () => {
  let component: AdminUsuarios;
  let fixture: ComponentFixture<AdminUsuarios>;
  let adminService: AdminService;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, CommonModule, AdminUsuarios]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminUsuarios);
    component = fixture.componentInstance;
    adminService = TestBed.inject(AdminService);
    fixture.detectChanges();
  });

  it('debería crearse', () => expect(component).toBeTruthy());

  it('debería cargar usuarios al inicializar', () => {
    spyOn(component, 'cargarUsuarios');
    component.ngOnInit();
    expect(component.cargarUsuarios).toHaveBeenCalled();
  });

  it('debería mostrar formulario al hacer clic en Agregar Usuario', () => {
    component.abrirFormulario();
    expect(component.mostrarForm).toBe(true);
  });

  it('debería cerrar formulario al cancelar', () => {
    component.mostrarForm = true;
    component.cerrarFormulario();
    expect(component.mostrarForm).toBe(false);
  });

  it('debería filtrar por email', () => {
    component.usuarios = [
      { id: 1, email: 'admin@derby.com', rol: 'Administrador' },
      { id: 2, email: 'arbitro@derby.com', rol: 'Arbitro' }
    ];
    component.busquedaEmail = 'admin';
    component.aplicarFiltro();
    expect(component.usuariosFiltrados.length).toBe(1);
    expect(component.usuariosFiltrados[0].email).toBe('admin@derby.com');
  });

  it('debería filtrar por rol', () => {
    component.usuarios = [
      { id: 1, email: 'admin@derby.com', rol: 'Administrador' },
      { id: 2, email: 'arbitro@derby.com', rol: 'Arbitro' },
      { id: 3, email: 'fan@derby.com', rol: 'Aficionado' }
    ];
    component.cambiarFiltro('Admin');
    expect(component.usuariosFiltrados.length).toBe(1);
    expect(component.usuariosFiltrados[0].rol).toBe('Administrador');
  });

  it('debería validar email requerido', () => {
    component.formulario.email = '';
    component.guardarUsuario();
    expect(component.notificacion.mostrar).toBe(true);
    expect(component.notificacion.tipo).toBe('error');
  });

  it('debería validar contraseña para nuevos usuarios', () => {
    component.editandoId = null;
    component.formulario.email = 'test@test.com';
    component.formulario.contrasena = '';
    component.guardarUsuario();
    expect(component.notificacion.mostrar).toBe(true);
  });

  it('debería resetear formulario después de crear usuario', () => {
    component.formulario = { email: 'test@test.com', contrasena: 'pwd', rol: 'Aficionado' };
    component.resetFormulario();
    expect(component.formulario.email).toBe('');
    expect(component.formulario.contrasena).toBe('');
  });

  it('debería cambiar a modo edición cuando se selecciona usuario', () => {
    const usuario = { id: 5, email: 'edit@test.com', rol: 'Arbitro' };
    component.abrirFormulario(usuario);
    expect(component.editandoId).toBe(5);
    expect(component.formulario.email).toBe('edit@test.com');
  });

  it('debería limpiar búsqueda cuando se cambia filtro de rol', () => {
    component.busquedaEmail = 'test';
    component.cambiarFiltro('Todos');
    component.aplicarFiltro();
    expect(component.filtroRol).toBe('Todos');
  });

  it('debería mostrar notificación de éxito', () => {
    component.mostrarAlerta('Usuario creado correctamente', 'exito');
    expect(component.notificacion.mostrar).toBe(true);
    expect(component.notificacion.tipo).toBe('exito');
    expect(component.notificacion.mensaje).toBe('Usuario creado correctamente');
  });

  it('debería mostrar notificación de error', () => {
    component.mostrarAlerta('Error al crear usuario', 'error');
    expect(component.notificacion.mostrar).toBe(true);
    expect(component.notificacion.tipo).toBe('error');
  });

  it('debería ocultarse después de 3 segundos', (done) => {
    component.mostrarAlerta('Test', 'exito');
    expect(component.notificacion.mostrar).toBe(true);
    setTimeout(() => {
      expect(component.notificacion.mostrar).toBe(false);
      done();
    }, 3100);
  });
});

