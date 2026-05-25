import { HttpInterceptorFn } from '@angular/common/http';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const usuarioJson = localStorage.getItem('usuarioActual');
  if (usuarioJson) {
    const usuario = JSON.parse(usuarioJson);
    if (usuario?.token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${usuario.token}`
        }
      });
    }
  }
  return next(req);
};
