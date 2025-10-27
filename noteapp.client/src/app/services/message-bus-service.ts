import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Note } from '../Models/Note';

@Injectable({
  providedIn: 'root'
})
export class MessageBusService {
  private messageSource = new BehaviorSubject<Note[]>([]); // default value
  message$ = this.messageSource.asObservable();

  sendMessage(message: Note[]) {
    this.messageSource.next(message);
  }
}
