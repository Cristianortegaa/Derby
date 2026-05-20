import { Component, OnInit } from '@angular/core';
import { RouterLink, Router } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, ReactiveFormsModule, CommonModule],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login implements OnInit {
  loginForm: FormGroup;
  registroForm: FormGroup;
  mostrarRegistro = false;
  showModal = false;
  modalMensaje = '';
  modalTipo = 'success';
  cargando = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router
  ) {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/), Validators.maxLength(50), Validators.minLength(5)]],
      contrasena: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(20), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/)]]
    });

    this.registroForm = this.fb.group({
      email: ['', [Validators.required, Validators.email, Validators.pattern(/^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/), Validators.maxLength(50), Validators.minLength(5)]],
      contrasena: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(20), Validators.pattern(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$/)]]
    });
  }

  ngOnInit() {
    if (this.authService.estaAutenticado()) {
      //this.router.navigate(['/']);
    }
  }

   onSubmitLogin() {
     if (this.loginForm.valid) {
       this.cargando = true;
       const datos = {
         email: this.loginForm.get('email')?.value,
         contrasena: this.loginForm.get('contrasena')?.value
       };

      this.authService.login(datos).subscribe({
        next: (usuario) => {
          this.cargando = false;
          this.modalMensaje = '¡Acceso Concedido!';
          this.modalTipo = 'success';
          this.showModal = true;
          setTimeout(() => {
            if (usuario.rol === 'Administrador') {
              this.router.navigate(['/admin']);
            } else if (usuario.rol === 'Arbitro') {
              this.router.navigate(['/arbitro']);
            } else {
              this.router.navigate(['/']);
            }
          }, 2000);
        },
        error: (error) => {
          this.cargando = false;
          const mensaje = error.error?.error || 'Error al iniciar sesión';
          this.modalMensaje = mensaje;
          this.modalTipo = 'error';
          this.showModal = true;
        }
      });
    } else {
      this.loginForm.markAllAsTouched();
    }
  }

   onSubmitRegistro() {
     if (this.registroForm.valid) {
       this.cargando = true;
       const datos = {
         email: this.registroForm.get('email')?.value,
         contrasena: this.registroForm.get('contrasena')?.value,
         rol: 'Aficionado'
       };

      this.authService.registro(datos).subscribe({
        next: (usuario: any) => {
          this.cargando = false;
          this.modalMensaje = '¡Registro Exitoso! Iniciando sesión...';
          this.modalTipo = 'success';
          this.showModal = true;
          setTimeout(() => {
            this.router.navigate(['/']);
          }, 2000);
        },
        error: (error: any) => {
          this.cargando = false;
          const mensaje = error.error?.error || 'Error al registrarse';
          this.modalMensaje = mensaje;
          this.modalTipo = 'error';
          this.showModal = true;
        }
      });
    } else {
      this.registroForm.markAllAsTouched();
    }
  }

  toggleRegistro() {
    this.mostrarRegistro = !this.mostrarRegistro;
    this.loginForm.reset();
    this.registroForm.reset();
  }

  closeModal() {
    this.showModal = false;
  }
}
