import { Component, OnInit } from '@angular/core';
import { UserService } from '../../_services/user.service';
import { User } from 'src/_models/user';
import { AlertifyService } from '../../_services/alertify.service';
import { ActivatedRoute } from '@angular/router';
import { Pagination, PaginatedResult } from 'src/_models/pagination';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css'],
})
export class MemberListComponent implements OnInit {
  users: User[];
  pagination: Pagination; // from the headers of the response which are stored on the PaginatedREsult returned from observable

  constructor(
    private userService: UserService,
    private alertify: AlertifyService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.users = data['users'].result; // the users on data(set in the resolver for the route) is now a paginatedResult of users which has the items in the result and the pagination info in the pagination prop.
      this.pagination = data['users'].pagination;
    });
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page; // comnes from ngx-bootstrap pagination component - this updates when the page number is clicked
    // fetch the users for the current page selected
    this.loadUsers();
  }

  loadUsers() {
    this.userService
      .getUsers(this.pagination.currentPage, this.pagination.itemsPerPage)
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
}
