import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Note } from '../Models/Note';

@Injectable({
  providedIn: 'root'
})
export class RefreshNotes {
  private messageSource = new BehaviorSubject<boolean>(true); // default value
  refreshhNotes$ = this.messageSource.asObservable();

  RefreshNotesMessage(refetchNotes: boolean) {
    this.messageSource.next(refetchNotes);
  }
}
