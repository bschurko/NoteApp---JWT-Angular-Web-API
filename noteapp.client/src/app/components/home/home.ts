import { Component, Injectable } from '@angular/core';
import { AuthenticationService } from '../../services/authentication.service';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  standalone: false,
  templateUrl: './home.html',
  styleUrl: './home.css',
})
export class Home {
  API_URL = environment.apiUrl;
  constructor(private authService: AuthenticationService, private http: HttpClient) { }

  logout(): void {
    this.authService.logout();
  }

  authorizedEndpoint(): void {
    var result = this.http.get<any>(`${environment.apiUrl}Account/GetAllUsers`)
    result.subscribe((cc: any) => console.log(cc));
    
  }

}
