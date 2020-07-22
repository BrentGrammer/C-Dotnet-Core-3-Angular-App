import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';

import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from 'src/_models/user';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User; // used to store user from response and get main photo url to show in nav.  This is also immediately populated in the app.component.ts class when app loads.
  constructor(private http: HttpClient) {}

  // store token from bavckend in localstorage
  login(model: any) {
    return this.http.post(this.baseUrl + 'login', model).pipe(
      map((response: any) => {
        const user = response;
        if (user) {
          localStorage.setItem('token', user.token);
          localStorage.setItem('user', JSON.stringify(user.user)); // store the user from res in localstorage to access the main photo url to show in navbar
          this.decodedToken = this.jwtHelper.decodeToken(user.token);

          this.currentUser = user.user;
        }
      })
    );
  }

  register(model: any) {
    return this.http.post(this.baseUrl + 'register', model);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
