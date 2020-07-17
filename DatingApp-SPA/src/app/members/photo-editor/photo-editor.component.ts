import { Component, OnInit, Input } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';

import { Photo } from 'src/_models/photo';
import { environment } from 'src/environments/environment';
import { AuthService } from 'src/app/_services/auth.service';

/**
 * Child of member-edit comonent used to upload photos
 */
@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css'],
})
export class PhotoEditorComponent implements OnInit {
  // bring in photos on the user object from parent component - use Input()
  @Input() photos: Photo[];

  // ng2-file-upload package setup - remember to import FileUploadModule from the package into the imports in app.module.ts
  uploader: FileUploader;
  hasBaseDropZoneOver = false;

  baseUrl = environment.apiUrl;

  constructor(private authService: AuthService) {}

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
}
