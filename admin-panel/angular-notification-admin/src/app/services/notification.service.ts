import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { NotificationRequest, NotificationResponse, ApiResponse } from '../models/notification.model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  sendNotification(request: NotificationRequest): Observable<ApiResponse<NotificationResponse>> {
    return this.http.post<ApiResponse<NotificationResponse>>(
      `${this.apiUrl}/notification/send`,
      request
    ).pipe(
      catchError(this.handleError)
    );
  }

  getAllNotifications(): Observable<ApiResponse<NotificationResponse[]>> {
    return this.http.get<ApiResponse<NotificationResponse[]>>(
      `${this.apiUrl}/notification`
    ).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'An unknown error occurred!';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      errorMessage = error.error?.message || `Server Error: ${error.status} - ${error.statusText}`;
    }
    console.error('NotificationService Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
