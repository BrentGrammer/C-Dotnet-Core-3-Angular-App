import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Message } from '../_models/message';
import { PaginatedResult, Pagination } from '../_models/pagination';
import { AlertifyService } from '../_services/alertify.service';
import { AuthService } from '../_services/auth.service';
import { UserService } from '../_services/user.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css'],
})
export class MessagesComponent implements OnInit {
  messages: Message[];
  pagination: Pagination;
  // from query param - can be set to Inbox/Outbox as well
  messageContainer = 'Unread';

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      this.messages = data['messages'].result; // the messages on data(set in the resolver for the route) is now a paginatedResult of messages which has the items in the result and the pagination info
      this.pagination = data['messages'].pagination;
    });
  }

  // resolver loads initial messages and then this is used when user changes page in pagination to get those messages
  loadMessages() {
    this.userService
      .getMessages(
        this.authService.decodedToken.nameid,
        this.pagination.currentPage,
        this.pagination.itemsPerPage,
        this.messageContainer
      )
      .subscribe(
        (res: PaginatedResult<Message[]>) => {
          this.messages = res.result;
          this.pagination = res.pagination;
        },
        (err) => {
          this.alertify.error(err);
        }
      );
  }

  pageChanged(event: any): void {
    this.pagination.currentPage = event.page;
    this.loadMessages();
  }
}
