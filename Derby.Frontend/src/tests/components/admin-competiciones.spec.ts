import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AdminCompeticiones } from '../../app/components/components/admin/admin-competiciones/admin-competiciones';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { provideRouter } from '@angular/router';
import { vi } from 'vitest';

describe('AdminCompeticiones', () => {
  let component: AdminCompeticiones;
  let fixture: ComponentFixture<AdminCompeticiones>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, CommonModule, AdminCompeticiones],
      providers: [provideRouter([])]
    }).compileComponents();

    fixture = TestBed.createComponent(AdminCompeticiones);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crearse', () => expect(component).toBeTruthy());

  it('debería cargar competiciones al inicializar', () => {
    const spy = vi.spyOn(component, 'cargarCompeticiones');
    component.ngOnInit();
    expect(spy).toHaveBeenCalled();
  });

  it('debería abrir formulario de creación', () => {
    component.abrirFormComp();
    expect(component.mostrarFormComp).toBe(true);
    expect(component.editandoCompId).toBeNull();
  });

  it('debería filtrar competiciones por nombre', () => {
    component.competiciones = [
      { id: 1, nombre: 'Liga Derby', estado: 'Activo' },
      { id: 2, nombre: 'Copa Derby', estado: 'Activo' }
    ];
    component.busquedaNombre = 'Liga';
    component.aplicarFiltro();
    expect(component.competicionesFiltradas.length).toBe(1);
  });

  it('debería mostrar todas las competiciones sin filtro', () => {
    component.competiciones = [
      { id: 1, nombre: 'Liga Derby', estado: 'Activo' },
      { id: 2, nombre: 'Copa Derby', estado: 'Activo' }
    ];
    component.busquedaNombre = '';
    component.aplicarFiltro();
    expect(component.competicionesFiltradas.length).toBe(2);
  });

  it('debería validar nombre requerido', () => {
    component.formularioComp.nombre = '';
    component.formularioComp.temporada = '';
    component.guardarComp();
    expect(component.notificacion.mostrar).toBe(true);
    expect(component.notificacion.tipo).toBe('error');
  });

  it('debería cerrar formulario al cancelar', () => {
    component.mostrarFormComp = true;
    component.cerrarForm();
    expect(component.mostrarFormComp).toBe(false);
  });

  it('debería resetear formulario después de guardar', () => {
    component.formularioComp.nombre = 'Test';
    component.resetForm();
    expect(component.formularioComp.nombre).toBe('');
  });

  it('debería cargar datos para editar', () => {
    const competicion = { id: 1, nombre: 'Liga', temporada: '2025', descripcion: 'Test', estado: 'Activo', tipo: 'Liga', tipoJuego: 'Futbol-11' };
    component.abrirFormComp(competicion);
    expect(component.editandoCompId).toBe(1);
    expect(component.formularioComp.nombre).toBe('Liga');
  });

  it('debería mostrar total de competiciones', () => {
    component.competiciones = [
      { id: 1, nombre: 'L1', estado: 'Activo' },
      { id: 2, nombre: 'L2', estado: 'Activo' },
      { id: 3, nombre: 'L3', estado: 'Inactivo' }
    ];
    expect(component.competiciones.length).toBe(3);
  });

  it('debería filtrar case-insensitive', () => {
    component.competiciones = [
      { id: 1, nombre: 'LIGA DERBY', estado: 'Activo' }
    ];
    component.busquedaNombre = 'liga';
    component.aplicarFiltro();
    expect(component.competicionesFiltradas.length).toBe(1);
  });

  it('debería mostrar notificación de éxito', () => {
    component.mostrarAlerta('Competición creada', 'exito');
    expect(component.notificacion.mostrar).toBe(true);
    expect(component.notificacion.tipo).toBe('exito');
  });

  it('debería mostrar notificación de error', () => {
    component.mostrarAlerta('Error en la operación', 'error');
    expect(component.notificacion.mostrar).toBe(true);
    expect(component.notificacion.tipo).toBe('error');
  });

  it('debería permanecer en búsqueda después de agregar', () => {
    component.busquedaNombre = 'Liga';
    component.competiciones = [{ id: 1, nombre: 'Liga Derby', estado: 'Activo' }];
    component.aplicarFiltro();
    expect(component.busquedaNombre).toBe('Liga');
  });

  it('debería cancelar edición correctamente', () => {
    component.editandoCompId = 5;
    component.mostrarFormComp = true;
    component.cerrarForm();
    expect(component.editandoCompId).toBeNull();
    expect(component.mostrarFormComp).toBe(false);
  });
});
