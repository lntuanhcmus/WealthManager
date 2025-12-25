import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../../../../../apps/wealth-manager-ui/src/environments/environment';
import { LoginRequest, AuthResponse } from '@wealth-manager/shared/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);

  private apiUrl = `${environment.apiUrl}/auth`;

  constructor() { }

  login(payload: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, payload).pipe(tap(response => {
      this.setSession(response);
    }));
  }

  private setSession(authResult: AuthResponse) {
    localStorage.setItem('token', authResult.jwToken);
    localStorage.setItem('refreshToken', authResult.refreshToken.token);

    const user = {
      id: authResult.id,
      name: authResult.userName,
      email: authResult.email,
      roles: authResult.roles
    };
    localStorage.setItem('user', JSON.stringify(user));
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
    localStorage.removeItem('refreshToken');
  }

  IsLoggedIn(): boolean {
    return !!localStorage.getItem('token');
  }
}
