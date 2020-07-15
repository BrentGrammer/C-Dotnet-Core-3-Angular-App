import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';

/**
 * Purpose of this route guard is to prevent the user from leaving the edit member form before they save changes and then lose their edits
 * remember to add the guard to the providers in app module and the route
 */
@Injectable()
export class PreventUnsavedChanges
  implements CanDeactivate<MemberEditComponent> {
  canDeactivate(component: MemberEditComponent) {
    if (component.editForm.dirty) {
      return confirm('Leave? Unsaved changes will be lost!');
    }
    //returning true means allow the route change to go through
    return true;
  }
}
