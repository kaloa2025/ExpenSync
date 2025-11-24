import { Component, EventEmitter, Output } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { NgIf } from '@angular/common';
import { Router } from '@angular/router';
import { OtpVerificationResponse, ResetPasswordResponse, VerifyEmailRequest, VerifyEmailResponse } from '../../../services/forgot-password/forgot-password.modals';
import { ForgotPasswordService } from '../../../services/forgot-password/forgot-password.service';
import { ForgotPasswordToastService } from '../../../services/forgot-password/forgot-password.toast.service';
@Component({
  selector: 'app-enter-email',
  imports: [ReactiveFormsModule, NgIf],
  templateUrl: './enter-email.html',
  styleUrl: './enter-email.css',
})
export class EnterEmail {

  form = new FormGroup({
    email : new FormControl('', [Validators.required, Validators.email])
  });

  constructor(private router : Router, private forgotPasswordService : ForgotPasswordService, private toastService : ForgotPasswordToastService){}

  onSubmit()
  {
    if(this.form.valid)
    {
      const req : VerifyEmailRequest = {
        email : this.form.get('email')?.value,
      };
      this.forgotPasswordService.verfiyEmailSendOtp(req).subscribe({
        next:(res)=>{

          if(!res || res.success == false)
          {
            this.toastService.show(res.message||"Can't generate Otp now.");
          }
          else
          {
            this.toastService.show("OTP sent on email if valid.");
            localStorage.setItem('otpExpSec', String(res.otpExpirySec));
            localStorage.setItem('email', res.email);
            localStorage.setItem('emailVerified', 'true');
            setTimeout(()=>{
              this.router.navigate(['forgot-password/otp-verification']);
            }, 2500);
          }
        },
        error:(err)=>{
          this.toastService.show("Error submitting email: "+err?.error?.message);
        }
      })
    }
    else
    {
      this.toastService.show("Please enter email.");
      return;
    }
  }
}
