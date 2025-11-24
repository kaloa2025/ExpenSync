import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ForgotPasswordToastService {
  private toastSubject = new Subject<string>();
  toast$ = this.toastSubject.asObservable();
  show(message:string)
  {
    this.toastSubject.next(message);
  }
}
