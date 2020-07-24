import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { JwtHelperService } from '@auth0/angular-jwt';

import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { User } from 'src/_models/user';
import { BehaviorSubject } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  baseUrl = environment.apiUrl + 'auth/';
  jwtHelper = new JwtHelperService();
  decodedToken: any;
  currentUser: User; // used to store user from response and get main photo url to show in nav.  This is also immediately populated in the app.component.ts class when app loads.
  // use a BehaviorSubject to store user main photo url to use in nav and other components - this subject can be passed vals (updated photo) and is an Observable - set an initial default pic
  photoUrl = new BehaviorSubject<string>('../../assets/user.png');
  currentPhoto = this.photoUrl.asObservable(); //unclear why this is done - the description says that this converts a subject to an observable you can subscrie to which abstracts something away
  // comoonents now subscribe to authService.currentPhoto and get the updated main photo
  // for example, the app.component.ts is using changememberphoto here to set this property on app load to the user's main photo on login

  constructor(private http: HttpClient) {}

  changeMemberPhoto(photoUrl: string) {
    this.photoUrl.next(photoUrl);
  }

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
          // emit the user's main photo to the behavior subject to set the user photo that other components can access through the currentPhoto which is populated this way
          // when the currentPhoto is updated, the components will update (subscribed to it)
          this.changeMemberPhoto(this.currentUser.photoUrl);
        }
      })
    );
  }

  register(user: User) {
    return this.http.post(this.baseUrl + 'register', user);
  }

  loggedIn() {
    const token = localStorage.getItem('token');
    return !this.jwtHelper.isTokenExpired(token);
  }
}
