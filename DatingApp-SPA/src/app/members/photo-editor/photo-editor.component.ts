import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

import { Photo } from 'src/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';
import { UserService } from 'src/app/_services/user.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

/**
 * Child of member-edit comonent used to upload photos
 */
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  // bring in photos on the user object from parent component(member-edit) - use Input()
  @Input() photos: Photo[];
  // use output prop to update the photo shown on the left as the new main photo when a new main photo is selected
  // this is used to emit the url to the member-edit parent component so it shows the new main photo
  @Output() getMemberPhotoChange = new EventEmitter<string>(); // emit the photo url string
  currentMain: Photo; // ref to the current main photo used to update the ui when a new main photo is set

  // ng2-file-upload package setup - remember to import FileUploadModule from the package into the imports in app.module.ts
  uploader: FileUploader;
  hasBaseDropZoneOver = false;

  baseUrl = environment.apiUrl;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private alertify: AlertifyService
  ) {}

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }

  // setup the ng2-file-upload uploader
  initializeUploader() {
    this.uploader = new FileUploader({
      url:
        this.baseUrl +
        'users/' +
        this.authService.decodedToken.nameid +
        '/photos',
      authToken: 'Bearer ' + localStorage.getItem('token'),
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });
    // Extend the initialization to say that the file does not have credentials (has to do with cookies) to prevent a cors error related to allowing origins in include mode
    // We are using Jwts and the credentials sent with the request are not needed for cookies
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    // use the callback from the uploader to add the photo to the photos array displayed when it is done uploading
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response); // response is a string and needs to be turned into an object
        const photo = {
          id: res.id,
          url: res.url,
          dateAdded: res.dateAdded,
          description: res.description,
          isMain: res.isMain,
        };
        this.photos.push(photo);
      }
    };
  }

  setMainPhoto(photo: Photo) {
    this.userService
      .setMainPhoto(this.authService.decodedToken.nameid, photo.id)
      .subscribe(
        () => {
          // get the updated current main photo and set the currentMain to false
          this.currentMain = this.photos.filter((p) => p.isMain === true)[0];
          this.currentMain.isMain = false;
          photo.isMain = true;
          // use authservice to update the behaviorsubject and emit the new photo to all subscribed and interested components
          this.authService.changeMemberPhoto(photo.url);
          // need to update the authservice user obj main photo and also local storage,so if browser is refreshed, then the token is updated.
          this.authService.currentUser.photoUrl = photo.url;
          localStorage.setItem(
            'user',
            JSON.stringify(this.authService.currentUser)
          );
        },
        (error) => {
          this.alertify.error(error);
        }
      );
  }

  deletePhoto(id: number) {
    this.alertify.confirm('Are you sure?', () => {
      this.userService
        .deletePhoto(this.authService.decodedToken.nameid, id)
        .subscribe(
          () => {
            // delete the photo from the frontend array now
            this.photos.splice(
              this.photos.findIndex((p) => p.id === id),
              1
            );
            this.alertify.success('Photo deleted.');
          },
          (error) => {
            this.alertify.error('Error deleting photo.');
          }
        );
    });
  }
}
