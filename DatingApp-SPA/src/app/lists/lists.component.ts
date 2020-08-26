import { Component, OnInit } from '@angular/core';
import { Pagination, PaginatedResult } from 'src/_models/pagination';
import { User } from 'src/_models/user';
import { AuthService } from '../_services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from '../_services/alertify.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css'],
})
export class ListsComponent implements OnInit {
  users: User[];
  pagination: Pagination;
  likesParam: string;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    // subscribe to the users data onh the route set by the listsresolver (assigned in the routes.ts file for lists component route)
    this.route.data.subscribe((data) => {
      this.users = data['users'].result; // the users on data(set in the resolver for the route) is now a paginatedResult of users which has the items in the result and the pagination info
      this.pagination = data['users'].pagination;
    });
    this.likesParam = 'Likers'; // this is set by a binding to the radio buttons in the html template when user clicks them
  }

  loadUsers() {
    this.userService
      .getUsers(
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        null,
        this.likesParam
      )
      .subscribe(
        (res: PaginatedResult<User[]>) => {
          this.users = res.result;
          this.pagination = res.pagination;
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page; // comnes from ngx-bootstrap pagination component - this updates when the page number is clicked
    // fetch the users for the current page selected
    this.loadUsers();
  }
}
