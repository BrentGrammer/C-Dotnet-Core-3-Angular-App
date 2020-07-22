import { Component, OnInit } from '@angular/core';
import { AuthService } from './_services/auth.service';
import { JwtHelperService } from '@auth0/angular-jwt';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  jwtHelper = new JwtHelperService();
  constructor(private authService: AuthService) {}

  // setting token up here on authservice ensures the decodedtoken info can b read by the app on refreshing the browser
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    const user = JSON.parse(localStorage.getItem('user'));
    if (token) {
      this.authService.decodedToken = this.jwtHelper.decodeToken(token);
    }
    // a user is also stored to access data such as main photo url to display in the nav on login.  Check if it is stored and populate the authservice current user prop with what is stored so other components can use ie
    if (user) {
      this.authService.currentUser = user;
      this.authService.changeMemberPhoto(user.photoUrl); // this sets the behaviorsubject in the auth service which other components are subscribed to with the main photo on app load
    }
  }
}
