import { Component, OnInit, Input } from '@angular/core';
import { Photo } from 'src/_models/photo';

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

  constructor() {}

  ngOnInit(): void {}
}
