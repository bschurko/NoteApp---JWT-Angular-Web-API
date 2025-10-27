import { INote, Note } from '../Models/Note';
import { HttpClient } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, shareReplay, tap } from 'rxjs/operators';
import { Component, Output, EventEmitter, OnInit, Injectable } from '@angular/core';
import { environment } from '../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NoteService {
  private API_URL: string = environment.apiUrl;
  private notes$!: Observable<Note[]>;

  constructor(private http: HttpClient, private query: QueryService) { }
   
  public getNotes(useCache = false): Observable<Note[]> {
    this.notes$ = this.query.Get<Note[]>(`${this.API_URL}Notes/GetNotes`, useCache);
    return this.notes$;
  }

  public addNote(note: Note): Observable<Note> {
    return this.query.post<Note>(this.API_URL + 'Notes/AddNote', note)
  }

  public deleteNote(id: number): Observable<Note> {
    return this.query.delete<Note>(this.API_URL + 'Notes/DeleteNote/' + id)
  }

  public updateNote(id: number, note: Note): Observable<Note> {
    return this.query.put<Note>(this.API_URL + 'Notes/UpdateNote/' + id, note);
  }

  public invalidate(url: string): void {
    this.query.invalidate(url);
  }

  public clearCache(): void {
    this.query.clearCache();
  }
}



@Injectable({
  providedIn: 'root'
})
export class QueryService {
  private cache = new Map<string, Observable<any>>();

  constructor(private http: HttpClient) { }

  put<T>(url: string, body: any, invalidateUrl?: string): Observable<T> {
    return this.http.put<T>(url, body).pipe(
      tap(() => {
        if (invalidateUrl) this.invalidate(invalidateUrl);
        console.log(`PUT to ${url}`);
      }),
      catchError(err => {
        console.error(`PUT error at ${url}`, err);
        return throwError(() => err);
      })
    );
  }


  delete<T>(url: string, invalidateUrl?: string): Observable<T> {
    return this.http.delete<T>(url).pipe(
      tap(() => {
        if (invalidateUrl) this.invalidate(invalidateUrl);
        console.log(`DELETE to ${url}`);
      }),
      catchError(err => {
        console.error(`DELETE error at ${url}`, err);
        return throwError(() => err);
      })
    );
  }

  post<T>(url: string, body: any): Observable<T> {
    return this.http.post<T>(url, body).pipe(
      tap(() => {
        console.log(`Posted to: ${url}`);
        this.invalidate(url); // Optional: clear cache for this endpoint
      }),
      catchError(err => {
        console.error(`POST error at ${url}`, err);
        return throwError(() => err);
      })
    );
  }

  Get<T>(url: string, useCache = true): Observable<T> {
    if (useCache && this.cache.has(url)) {
      return this.cache.get(url)!;
    }

    const request$ = this.http.get<T>(url).pipe(
      shareReplay(1),
      catchError(err => {
        this.cache.delete(url);
        return throwError(() => err);
      }),
      tap(() => console.log(`Fetched: ${url}`))
    );

    if (useCache) {
      this.cache.set(url, request$);
    }

    return request$;
  }

  invalidate(url: string): void {
    this.cache.delete(url);
  }

  clearCache(): void {
    this.cache.clear();
  }
}
