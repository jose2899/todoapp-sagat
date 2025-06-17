import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CommentService {
  private apiUrl = 'https://localhost:7037/api/tasks';

  constructor(private http: HttpClient) {}

  getComments(taskId: number): Observable<any[]> {
    return this.http.get<any[]>(`${this.apiUrl}/${taskId}/comments`);
  }

  createComment(taskId: number, comment: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${taskId}/comments`, comment);
  }

  updateComment(taskId: number, comment: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${taskId}/comments`, comment);
  }

  deleteComment(taskId: number, comment: any): Observable<any> {
    return this.http.post<any>(`${this.apiUrl}/${taskId}/comments`, comment);
  }

  // otros métodos como actualizar, eliminar, etc.
}
