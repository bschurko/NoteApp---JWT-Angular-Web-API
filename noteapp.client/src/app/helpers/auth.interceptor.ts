// auth.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpRequest, HttpHandler, HttpEvent, HttpInterceptor } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { Observable, of, throwError } from 'rxjs';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    try {

      console.log(`[HTTP - Interceptor] ${request.method} → ${request.url}`);

      const isPublic = [
        '/Account/Login',
        '/Account/Register',
        '/assets/',
      ].some(url => request.url.includes(url));

      if (isPublic) {
        return next.handle(request); 
      }

      const user = JSON.parse(localStorage.getItem('user')!);
      const token = user?.accessToken;

      if (token && token.length > 10) {
            request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${token}`
          }
        });
      }

      return next.handle(request).pipe(
        catchError(err => {
          console.error('HTTP Error:', err);
          return throwError(() => err);
        })
      );

    } catch (error) {
      console.log(error);
      return next.handle(request);
    }
  }
}
