import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ReplaySubject } from 'rxjs';
import { map } from 'rxjs/operators';
import { User } from '../_models/user';
map

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = 'https://localhost:5001/api/';
  private currentUserSource = new ReplaySubject<User>(1);
  currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) { }

  Register(model: any) {
    return this.http.post(this.baseUrl + 'Account/register', model).pipe(
      map((response: User) => {
        if (response) {
          localStorage.setItem('user', JSON.stringify(response));
          this.currentUserSource.next(response);
        }
      })
    );
  }

  Login(model: any) {
    return this.http.post(this.baseUrl + 'Account/login', model).pipe(
      map((response: User) => {
        if (response) {
          localStorage.setItem('user', JSON.stringify(response));
          this.currentUserSource.next(response);
        }
      })
    );
  }
  Logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
  }

  SetCurrentUser(user: User) {
    this.currentUserSource.next(user);
  }
}