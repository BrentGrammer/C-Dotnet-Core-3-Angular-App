import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from 'src/_models/user';
import { AuthService } from '../_services/auth.service';

// purpose of this resolver file is to prevent the loading of member detail component before data has been fetched
// otherwise you would need ? operators on all the data in the html template to prevent errors
// remember to add this resolver to the routes and app modules

@Injectable()
export class MemberEditResolver implements Resolve<User> {
  constructor(
    private userService: UserService,
    private router: Router,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  resolve(): Observable<User> {
    // the user id is taken from the auth token and not the route param as a url query param as in the other resolver for viewing a member detail.
    return this.userService.getUser(this.authService.decodedToken.nameid).pipe(
      catchError((error) => {
        this.alertify.error('Problem getting your data');
        this.router.navigate(['/members']);
        // need to return an Observable of null on error - do this using of from rxjs:
        return of(null);
      })
    );
  }
}
//
