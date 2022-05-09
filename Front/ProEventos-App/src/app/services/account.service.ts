import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { User } from '../models/identity/User';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { UserUpdate } from '@app/models/identity/UserUpdate';

@Injectable()
export class AccountService {
  baseURL = 'api/account';
  private currentUserSource = new BehaviorSubject<User>(
    JSON.parse(localStorage.getItem('user')!) as User
  );
  public currentUser$ = this.currentUserSource.asObservable();

  constructor(private http: HttpClient) {}

  public login(model: any): Observable<void> {
    return this.http.post<User>(`${this.baseURL}/login`, model).pipe(
      take(1),
      map((response: User) => {
        if (response) {
          this.setCurrentUser(response);
        }
      })
    );
  }

  logout(): void {
    localStorage.removeItem('user');
    this.currentUserSource.next(null!);
    this.currentUserSource.complete();
  }

  public setCurrentUser(user: User): void {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUserSource.next(user);
  }

  getUser(): Observable<UserUpdate> {
    return this.http.get<UserUpdate>(`${this.baseURL}/getUser`).pipe(take(1));
  }

  updateUser(model: UserUpdate): Observable<void> {
    return this.http.put<UserUpdate>(`${this.baseURL}/updateUser`, model).pipe(
      take(1),
      map((user: UserUpdate) => {
        this.setCurrentUser(user);
      })
    );
  }

  public register(model: any): Observable<void> {
    return this.http.post<User>(`${this.baseURL}/register`, model).pipe(
      take(1),
      map((response: User) => {
        if (response) {
          this.setCurrentUser(response);
        }
      })
    );
  }

  postUpload(file: File): Observable<UserUpdate> {
    const formData = new FormData();
    formData.append('file', file);

    return this.http
      .post<UserUpdate>(`${this.baseURL}/upload-image`, formData)
      .pipe(take(1));
  }
}
