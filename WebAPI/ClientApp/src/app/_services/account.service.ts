import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { map, finalize, catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Account } from '@app/_models';
import { throwError } from 'rxjs';
import { AlertService } from './alert.service';

const baseUrl = `${environment.apiUrl}/Account`;

@Injectable({ providedIn: 'root' })
export class AccountService {
    private accountSubject: BehaviorSubject<Account | null>;
    public account: Observable<Account | null>;

    constructor(
        private router: Router,
        private http: HttpClient,
        private alertService: AlertService
    ) {
        
        this.accountSubject = new BehaviorSubject<Account | null>(null);
        this.account = this.accountSubject.asObservable();
    }

    public get accountValue() {
        
        return this.accountSubject.value;
    }

  login(email: string, password: string) {
    debugger
    return this.http.post<any>(`${baseUrl}/Login`, { email, password })  //, { withCredentials: true }
      .pipe(map(account => {

        if (!account.success) {  //&& account.success != undefined
          return account;//this.alertService.warn(account.message); //
        }
        else {
          this.accountSubject.next(account);
          this.startRefreshTokenTimer();
          console.log(account, 'account');
          return account;
        }
     }), catchError(this.handleError));
    }

    logout()
     {
        
        this.http.post<any>(`${baseUrl}/revoke-token`, {}).subscribe();//, { withCredentials: true }
        this.stopRefreshTokenTimer();
        this.accountSubject.next(null);
        this.router.navigate(['/account/login']);
    }

    refreshToken() {
      return this.http.post<any>(`${baseUrl}/RefreshToken`, {}) //, { withCredentials: true }
            .pipe(map((account) => {
                this.accountSubject.next(account);
                this.startRefreshTokenTimer();
                return account;
            }));
    }


  //constructor(private http: HttpClient) { }

  //register(model: RegisterModel): Observable<any> {
  //  return this.http.post<any>(`${this.apiUrl}/register`, model);
  //}

  register(account: Account) {
    
        return this.http.post(`${baseUrl}/Register`, account);
    }

    verifyEmail(token: string) {
        return this.http.post(`${baseUrl}/verify-email`, { token });
    }

    forgotPassword(email: string) {
        return this.http.post(`${baseUrl}/forgot-password`, { email });
    }

    validateResetToken(token: string) {
        return this.http.post(`${baseUrl}/validate-reset-token`, { token });
    }

    resetPassword(token: string, password: string, confirmPassword: string) {
        return this.http.post(`${baseUrl}/reset-password`, { token, password, confirmPassword });
    }

    getAll() {
        return this.http.get<Account[]>(baseUrl);
    }

    getById(id: string) {
        return this.http.get<Account>(`${baseUrl}/${id}`);
    }

    create(params: any) {
        return this.http.post(baseUrl, params);
    }

    update(id: string, params: any) {
        return this.http.put(`${baseUrl}/${id}`, params)
            .pipe(map((account: any) => {
                // update the current account if it was updated
                if (account.id === this.accountValue?.id) {
                    // publish updated account to subscribers
                    account = { ...this.accountValue, ...account };
                    this.accountSubject.next(account);
                }
                return account;
            }));
    }

    delete(id: string) {
        return this.http.delete(`${baseUrl}/${id}`)
            .pipe(finalize(() => {
                // auto logout if the logged in account was deleted
                if (id === this.accountValue?.id)
                    this.logout();
            }));
    }

    // helper methods

    private refreshTokenTimeout?: any;

  private startRefreshTokenTimer() {
    debugger;
    // parse json object from base64 encoded jwt token
    const jwtBase64 = this.accountValue!.jwtToken!.split('.')[1];
        const jwtToken = JSON.parse(atob(jwtBase64));

        // set a timeout to refresh the token a minute before it expires
        const expires = new Date(jwtToken.exp * 1000);
        const timeout = expires.getTime() - Date.now() - (60 * 1000);
        this.refreshTokenTimeout = setTimeout(() => this.refreshToken().subscribe(), timeout);
    }

    private stopRefreshTokenTimer() {
        clearTimeout(this.refreshTokenTimeout);
    }

  private handleError(error: HttpErrorResponse) {
    debugger;
    let errorMessage = 'Unknown error!';
    if (error.error instanceof ErrorEvent) {
      // Client-side errors
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side errors
      if (error.status === 401) {
        errorMessage = 'Username or password is incorrect';
      } else {
        errorMessage = `${error}`;  //Error Code: ${error.status}\nMessage:
      }
    }
    return throwError(errorMessage);
  }
}
