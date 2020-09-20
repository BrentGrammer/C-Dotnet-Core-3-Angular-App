import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from 'src/app/_models/user';
import { PaginatedResult } from 'src/app/_models/pagination';
import { map } from 'rxjs/operators';
import { Message } from '../_models/message';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  // using the value from the environments folder created by Andular CLI:
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  // getUsers takes optional paramns which are used in the query for pagination - the api defaults to page 1 and ten results if no params are in the query string
  // this method is called and used in the memberslist-resolver component
  getUsers(
    page?,
    itemsPerPage?,
    userParams?,
    likesParam? // likesParam is part of userParams, but pass it in separately here as an optional param
  ): Observable<PaginatedResult<User[]>> {
    // paginated class holds the list of items in the result prop and the pagination info as well
    const paginatedResult: PaginatedResult<User[]> = new PaginatedResult<
      User[]
    >();

    let params = new HttpParams();

    // check if optional params are passed in and set them on the query string with params.append
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    //userParams for filtering etc.
    if (userParams != null) {
      params = params.append('minAge', userParams.minAge);
      params = params.append('maxAge', userParams.maxAge);
      params = params.append('gender', userParams.gender);
      params = params.append('orderBy', userParams.orderBy);
    }

    if (likesParam === 'Likers') {
      params = params.append('likers', 'true');
    }

    if (likesParam === 'Likees') {
      params = params.append('likees', 'true');
    }

    // http Obersvables observe the response body by default but we need access to the pagination headers. So we use the observe override to look at the whole response
    // you need to do something with the response with headers etc. so use the rxjs pipe operator
    return this.http
      .get<User[]>(this.baseUrl + 'users', { observe: 'response', params })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination')) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            ); // theheaders is a string so you need to set it to js object
          }
          // map returns on every event emitted by observable
          return paginatedResult;
        })
      );
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + 'users/' + id);
  }

  updateUser(id: number, user: User) {
    return this.http.put(`${this.baseUrl}users/${id}`, user);
  }

  setMainPhoto(userId: number, id: number) {
    // pass in empty object to satisfy post body requirements - it is not used in the backend and the params for the post are pulled from the route params here
    return this.http.post(
      `${this.baseUrl}users/${userId}/photos/${id}/setMain`,
      {}
    );
  }

  deletePhoto(userId: number, id: number) {
    return this.http.delete(`${this.baseUrl}users/${userId}/photos/${id}`);
  }

  sendLike(id: number, recipientId: number) {
    //because this is a post you need to send something in the body, so just send an empty object
    return this.http.post(`${this.baseUrl}users/${id}/like/${recipientId}`, {});
  }

  getMessages(id: number, page?, itemsPerPage?, messageContainer?) {
    const paginatedResult: PaginatedResult<Message[]> = new PaginatedResult<
      Message[]
    >();

    let params = new HttpParams();

    params = params.append('messageContainer', messageContainer);
    // check if optional params are passed in and set them on the query string with params.append
    if (page != null && itemsPerPage != null) {
      params = params.append('pageNumber', page);
      params = params.append('pageSize', itemsPerPage);
    }

    // observe the headers in the response to get access to pagination info
    return this.http
      .get<Message[]>(this.baseUrl + 'users/' + id + '/messages', {
        observe: 'response',
        params,
      })
      .pipe(
        map((response) => {
          paginatedResult.result = response.body;
          if (response.headers.get('Pagination') !== null) {
            paginatedResult.pagination = JSON.parse(
              response.headers.get('Pagination')
            );
          }

          return paginatedResult;
        })
      );
  }
}
