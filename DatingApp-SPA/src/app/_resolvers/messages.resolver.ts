import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { UserService } from '../_services/user.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';

// remember to add this resolver to the routes and app modules

@Injectable()
export class MessagesResolver implements Resolve<Message[]> {
  // params that match to pagination values used in the headers
  pageNumber = 1;
  pageSize = 5;
  // matches to query string param for inbox/outbox
  messageContainer = 'Unread';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private router: Router,
    private alertify: AlertifyService
  ) {}

  resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
    // resolve automatically subscribes to method call here, but you still need to catch the error manually if one occurs
    return this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pageNumber,
        this.pageSize,
        this.messageContainer
      )
      .pipe(
        catchError((error) => {
          this.alertify.error('Problem getting messages');
          this.router.navigate(['/home']);
          // need to return an Observable of null on error - do this using of from rxjs:
          return of(null);
        })
      );
  }
}
//
