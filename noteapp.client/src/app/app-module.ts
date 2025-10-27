import { HttpClientModule } from '@angular/common/http';
import { NgModule, provideBrowserGlobalErrorListeners, Injectable } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormBuilder, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AppRoutingModule } from './app-routing-module';
import { App } from './app';
import { EditNotes } from './components/edit-notes/edit-notes';
import { NotesList } from './components/notes-list/notes-list';
import { SearchNotes } from './components/search-notes/search-notes';
import { FilterNotes } from './components/filter-notes/filter-notes';
import { Login } from './components/login/login';
import { Home } from './components/home/home';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from './helpers/auth.interceptor';
import { provideHttpClient, withInterceptors } from '@angular/common/http';



@NgModule({
  declarations: [
    App,
    NotesList,
    SearchNotes,
    FilterNotes,
    EditNotes,
    Login,
    Home
  ],
  imports: [
    BrowserModule, HttpClientModule,
    AppRoutingModule, FormsModule, ReactiveFormsModule
  ],
  providers: [
    provideBrowserGlobalErrorListeners(),
 
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [App]
})
export class AppModule { }
