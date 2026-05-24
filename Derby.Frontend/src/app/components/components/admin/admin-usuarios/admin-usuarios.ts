import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NavbarComponent } from '../../../navbar/navbar.component';
import { AdminService } from '../../../../services/admin.service';

@Component({
  selector: 'app-admin-usuarios',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NavbarComponent],
  templateUrl: './admin-usuarios.html',
  styleUrl: './admin-usuarios.css'
})
export class AdminUsuarios implements OnInit {
  usuarios: any[] = [];
  usuariosFiltrados: any[] = [];
  cargando = false;
  mostrarForm = false;
  editandoId: number | null = null;
  roles = ['Administrador', 'Arbitro', 'Aficionado'];
  filtroRol: string = 'Todos';
  busquedaEmail: string = '';

  notificacion = {
    mostrar: false,
    mensaje: '',
    tipo: 'exito'
  };

  modalConfirm = { mostrar: false, titulo: '', mensaje: '', textoConfirmar: 'Eliminar', onConfirm: () => {} };

  abrirConfirm(titulo: string, mensaje: string, accion: () => void, textoConfirmar = 'Eliminar') {
    this.modalConfirm = { mostrar: true, titulo, mensaje, textoConfirmar, onConfirm: accion };
  }
  cerrarConfirm() { this.modalConfirm.mostrar = false; }
  confirmar() { this.modalConfirm.onConfirm(); this.cerrarConfirm(); }

  formulario = {
    email: '',
    contrasena: '',
    rol: 'Aficionado',
    nombre: '',
    apellidos: ''
  };

  constructor(
    private adminService: AdminService,
    private cdr: ChangeDetectorRef
  ) {}

  ngOnInit() {
    this.cargarUsuarios();
  }

  mostrarAlerta(mensaje: string, tipo: 'exito' | 'error' = 'exito') {
    this.notificacion = { mostrar: true, mensaje, tipo };
    this.cdr.detectChanges();

    setTimeout(() => {
      this.notificacion.mostrar = false;
      this.cdr.detectChanges();
    }, 3000);
  }

  cargarUsuarios() {
    this.cargando = true;
    this.adminService.obtenerUsuarios().subscribe({
      next: (data) => {
        this.usuarios = data;
        this.aplicarFiltro();
        this.cargando = false;
        this.cdr.detectChanges();
      },
      error: (error) => {
        console.error('Error cargando usuarios:', error);
        this.cargando = false;
        this.cdr.detectChanges();
      }
    });
  }

  aplicarFiltro() {
    let filtrados = this.usuarios;

    // Filtrar por rol
    if (this.filtroRol !== 'Todos') {
      filtrados = filtrados.filter(u => u.rol === this.filtroRol);
    }

    // Filtrar por email
    if (this.busquedaEmail.trim()) {
      filtrados = filtrados.filter(u =>
        u.email.toLowerCase().includes(this.busquedaEmail.toLowerCase())
      );
    }

    this.usuariosFiltrados = filtrados;
    this.cdr.detectChanges();
  }

  cambiarFiltro(rol: string) {
    this.filtroRol = rol;
    this.aplicarFiltro();
  }

  abrirFormulario(usuario?: any) {
    if (usuario) {
      this.editandoId = usuario.id;
      this.formulario = {
        email: usuario.email,
        contrasena: '',
        rol: usuario.rol,
        nombre: usuario.nombreArbitro?.split(' ')[0] || '',
        apellidos: usuario.nombreArbitro?.split(' ').slice(1).join(' ') || ''
      };
    } else {
      this.editandoId = null;
      this.resetFormulario();
    }
    this.mostrarForm = true;
    setTimeout(() => { document.documentElement.scrollTop = 0; document.body.scrollTop = 0; }, 50);
  }

  guardarUsuario() {
    if (!this.formulario.email) {
      this.mostrarAlerta('El email es requerido', 'error');
      return;
    }

    if (this.editandoId === null && !this.formulario.contrasena) {
      this.mostrarAlerta('La contraseña es requerida para nuevos usuarios', 'error');
      return;
    }

    const datos = this.formulario;

    if (this.editandoId) {
      // Actualizar usuario existente
      this.adminService.actualizarUsuario(this.editandoId, datos).subscribe({
        next: (response) => {
          this.mostrarForm = false;
          this.editandoId = null;
          this.resetFormulario();
          this.cargarUsuarios();
          this.mostrarAlerta('Usuario actualizado correctamente');
        },
        error: (error) => {
          console.error('Error actualizando usuario:', error);
          this.mostrarAlerta('Error: ' + (error?.error?.error || error?.message), 'error');
        }
      });
    } else {
      // Crear nuevo usuario
      this.adminService.crearUsuario(datos).subscribe({
        next: (response) => {
          this.mostrarForm = false;
          this.resetFormulario();
          this.cargarUsuarios();
          this.mostrarAlerta('¡Usuario creado correctamente!');
        },
        error: (error) => {
          console.error('Error creando usuario:', error);
          this.mostrarAlerta('Error: ' + (error?.error?.error || error?.message), 'error');
        }
      });
    }
  }

  eliminarUsuario(id: number, email: string) {
    this.abrirConfirm('Eliminar usuario', `¿Estás seguro de eliminar el usuario ${email}?`, () => {
      this.adminService.eliminarUsuario(id).subscribe({
        next: () => {
          this.cargarUsuarios();
          this.mostrarAlerta('Usuario eliminado del sistema');
        },
        error: (error) => {
          console.error('Error eliminando usuario:', error);
          this.mostrarAlerta('Error al eliminar el usuario', 'error');
        }
      });
    });
  }

  cerrarFormulario() {
    this.mostrarForm = false;
    this.editandoId = null;
    this.resetFormulario();
  }

  resetFormulario() {
    this.formulario = {
      email: '',
      contrasena: '',
      rol: 'Aficionado',
      nombre: '',
      apellidos: ''
    };
  }
}
