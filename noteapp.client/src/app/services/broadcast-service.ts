 import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { Note } from '../Models/Note';

@Injectable({
  providedIn: 'root'
})
export class BroadcastService<T> {

  //private subject: BehaviorSubject<T>;

  //constructor(initialValue: T) {
  //  this.subject = new BehaviorSubject<T>(initialValue);
  //}


  // Observable for subscription (asObservable hides next/error/complete)
  //get getState(): Observable<T> {
  //  return this.subject.asObservable();
  //}

  //// Method to update the state
  //setState(value: T) {
  //  this.subject.next(value);
  //}

  //// Get current value
  //get value(): T {
  //  return this.subject.value;
  //}

  private broadcastSource = new BehaviorSubject<any>({}); // default value
  //  subscribe to this observable to get the messages  
  sourceMessage = this.broadcastSource.asObservable();

  // Emit a message to all subscribers
  broadcastMessage(message: T) {
    this.broadcastSource.next(message);
  }
}
