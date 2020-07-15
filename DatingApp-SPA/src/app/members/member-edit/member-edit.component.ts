import { Component, OnInit, ViewChild } from '@angular/core';
import { User } from 'src/_models/user';
import { ActivatedRoute } from '@angular/router';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  // use ViewChild to access elements or compononets in your html template view for this component
  @ViewChild('editForm') editForm: NgForm;
  user: User;

  constructor(
    private route: ActivatedRoute,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    this.route.data.subscribe((data) => {
      // user key is defined in the routes.ts module as the resolve data key populated by the resolver
      this.user = data['user'];
    });
  }

  updateUser() {
    console.log(this.user);
    this.alertify.success('Changes Saved!');
    // reset makes form pristine and clears values to remove info and disable save button. pass in the data for the user to repopulate the form values
    this.editForm.reset(this.user);
  }
}
