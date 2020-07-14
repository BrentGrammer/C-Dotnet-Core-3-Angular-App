import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from 'src/_models/user';

// purpose of this resolver file is to prevent the loading of member detail component before data has been fetched
// otherwise you would need ? operators on all the data in the html template to prevent errors
// remember to add this resolver to the routes and app modules

@Injectable()
export class MemberDetailResolver implements Resolve<User> {
  constructor(
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User> {
    // resolve automatically subscribes to method call here, but you still need to catch the error manually if one occurs
    return this.userService.getUser(route.params['id']).pipe(
      catchError((error) => {
        this.alertify.error('Problem getting data');
        this.router.navigate(['/members']);
        // need to return an Observable of null on error - do this using of from rxjs:
        return of(null);
      })
    );
  }
}
//
