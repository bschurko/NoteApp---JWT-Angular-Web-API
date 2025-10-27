 import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Note } from '../Models/Note';

@Injectable({
  providedIn: 'root'
})
export class BroadcastService {
  private broadcastSource = new BehaviorSubject<Note>(new Note()); // default value
  sourceMessage = this.broadcastSource.asObservable();

  broadcastMessage(message: Note) {
    this.broadcastSource.next(message);
  }
}
