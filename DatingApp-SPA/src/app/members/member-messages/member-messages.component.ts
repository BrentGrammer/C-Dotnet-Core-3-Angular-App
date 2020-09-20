import { Component, Input, OnInit } from '@angular/core';
import { tap } from 'rxjs/operators';
import { Message } from 'src/app/_models/message';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-member-messages',
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
})
export class MemberMessagesComponent implements OnInit {
  // to get the recipient id for showing message thread, we can pass down the user id in the parent component(member-detail)
  @Input() recipientId: number;
  messages: Message[];
  newMessage: any = {};

  constructor(
    private userService: UserService,
    private authService: AuthService,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages() {
    const currentUserId = +this.authService.decodedToken.nameid;

    this.userService
      .getMessageThread(currentUserId, this.recipientId)
      .pipe(
        // tap allows us to do something before subscribing
        tap((messages) => {
          messages.forEach((message) => {
            if (
              message.isRead === false &&
              message.recipientId === currentUserId // only mark read for current recipient looking at it
            ) {
              this.userService.markAsRead(currentUserId, message.id);
            }
          });
        })
      )
      .subscribe(
        (messages) => {
          this.messages = messages;
        },
        (err) => {
          this.alertify.error(err);
        }
      );
  }

  sendMessage() {
    // set this to the user id of the member detail profile that is open
    // you have access to that via an Input recipientId coming from the parent member-detail component
    this.newMessage.recipientId = this.recipientId;
    this.userService
      .sendMessage(this.authService.decodedToken.nameid, this.newMessage)
      .subscribe(
        (message: Message) => {
          // add to start of messages array
          this.messages.unshift(message);
          this.newMessage.content = ''; // reset newMEssage to empty so form does not populate
        },
        (err) => {
          this.alertify.error(err);
        }
      );
  }
}
