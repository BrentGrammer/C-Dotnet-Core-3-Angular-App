import { Component, OnInit } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(
    public authService: AuthService,
    private alertify: AlertifyService,
    private router: Router
  ) {}

  ngOnInit(): void {}

  login() {
    this.authService.login(this.model).subscribe(
      (next) => {
        this.alertify.success('logged in');
      },
      (error) => {
        this.alertify.error(error);
      },
      () => {
        //this is the complete pareameter you can use in subscribe()
        this.router.navigate(['/members']);
      }
    );
  }

  loggedIn() {
    return this.authService.loggedIn();
  }

  logout() {
    localStorage.removeItem('user');
    localStorage.removeItem('token');
    this.authService.currentUser = null;
    this.authService.decodedToken = null;
    this.alertify.message('logged out');
    this.router.navigate(['/home']);
  }
}
