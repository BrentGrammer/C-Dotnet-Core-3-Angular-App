import { Component, OnInit, ViewChild, HostListener } from '@angular/core';
import { User } from 'src/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  // use ViewChild to access elements or compononets in your html template view for this component
  @ViewChild('editForm') editForm: NgForm;
  user: User; // this is passed to the child photo-edit component so that it can accesss the photos
  photoUrl: string;
  // host listener prevents user from closing window or tab of browser before saving changes and prompts them with a warnging
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    //this causes a browser builtin popup to show if the form has been touched - you have no control over the styling etc. it is just part of the browser but offers a way to warn the user if they close a tab in the browser
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService,
    private authService: AuthService,
    private userService: UserService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      // user key is defined in the routes.ts module as the resolve data key populated by the resolver
      this.user = data['user'];
    });
    this.authService.currentPhoto.subscribe(
      (photoUrl) => (this.photoUrl = photoUrl)
    );
  }

  updateUser() {
    this.userService
      .updateUser(this.authService.decodedToken.nameid, this.user)
      .subscribe(
        (next) => {
          this.alertify.success('Changes Saved!');
          // reset makes form pristine and clears values to remove info and disable save button. pass in the data for the user to repopulate the form values
          this.editForm.reset(this.user);
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  updateMainPhoto(photoUrl) {
    // this is called when the output prop getChangeMemberphoto is emitted from photo-edit with the new url to the new main photo.
    this.user.photoUrl = photoUrl;
  }
}
