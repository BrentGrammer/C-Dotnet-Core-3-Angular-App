import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { User } from 'src/_models/user';

// remember to add this resolver to the routes and app modules

@Injectable()
export class MemberListResolver implements Resolve<User[]> {
  pageNumber = 1;
  pageSize = 5;

  constructor(
    private userService: UserService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<User[]> {
    // resolve automatically subscribes to method call here, but you still need to catch the error manually if one occurs
    return this.userService.getUsers(this.pageNumber, this.pageSize).pipe(
      catchError((error) => {
        this.alertify.error('Problem getting data');
        this.router.navigate(['/home']);
        // need to return an Observable of null on error - do this using of from rxjs:
        return of(null);
      })
    );
  }
}
//
