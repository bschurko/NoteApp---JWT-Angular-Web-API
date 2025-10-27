import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';
import { User } from '../Models/user';

@Injectable({ providedIn: 'root' })
export class UserService {
  constructor(private http: HttpClient) { }

  user: User | null = {
    id: 1,
    username: 'bschurko',
    firstName: 'Brett',
    lastName: 'Schurko',
    token: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJic2NodXJrbyIsImp0aSI6IjE5MTNhYzQ2LWFjZDEtNDkxNS1hNzg3LTk4YTJmZTdkNjMxNSIsImV4cCI6MTc2MTQzNjkwNSwiaXNzIjoiaHR0cHM6Ly9sb2NhbGhvc3Q6MzI3ODMifQ.dxAg60S04n0F9KzuoO244X30fhH4tY76mUwF5vzasY4'
  }

  getAll() {
    return this.user;
  }
}
