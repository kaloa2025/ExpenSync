import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { OtpVerificationResponse, ResetPasswordResponse, VerifyEmailResponse } from './forgot-password.modals';

@Injectable({
  providedIn: 'root'
})
export class ForgotPasswordService {

  private apiUrl = 'https://localhost:7253/api/forgotPassword';

  constructor(private httpClient : HttpClient){}

  verfiyEmailSendOtp(req:any):Observable<VerifyEmailResponse>
  {
    return this.httpClient.post<VerifyEmailResponse>(`${this.apiUrl}/verify-email`,req);
  }

  verifyOtp(req:any):Observable<OtpVerificationResponse>
  {
    return this.httpClient.post<OtpVerificationResponse>(`${this.apiUrl}/verify-otp`,req);
  }

  resetPassword(req:any):Observable<ResetPasswordResponse>
  {
    return this.httpClient.put<ResetPasswordResponse>(`${this.apiUrl}/reset-new-password`,req);
  }

  resendOTP(req:any):Observable<VerifyEmailResponse>
  {
    return this.httpClient.post<VerifyEmailResponse>(`${this.apiUrl}/resend-otp`,req);
  }

}
