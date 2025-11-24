import { Component, OnInit, OnDestroy, Output, EventEmitter } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { ForgotPasswordService } from '../../../services/forgot-password/forgot-password.service';
import { ForgotPasswordToastService } from '../../../services/forgot-password/forgot-password.toast.service';
import { OtpVerificationRequest, VerifyEmailRequest } from '../../../services/forgot-password/forgot-password.modals';
import { timeInterval } from 'rxjs';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-otp-verification',
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './otp-verification.html',
  styleUrl: './otp-verification.css',
})
export class OtpVerification implements OnInit, OnDestroy {

  isLoading : boolean = false;
  loading_message : string ="";

  form!: FormGroup;
  email: string | null = localStorage.getItem('email');
  expiryValue = localStorage.getItem('otpExpSec');
  timer: number = this.expiryValue?Number(this.expiryValue): 30;
  canResend: boolean = false;
  private timerInterval: any;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private forgotPasswordService : ForgotPasswordService,
    private toastService : ForgotPasswordToastService
  ) {
    this.form = this.fb.group({
      digit1: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit2: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit3: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
      digit4: ['', [Validators.required, Validators.pattern(/^[0-9]$/)]],
    });

    this.checkDetails();
  }

  checkDetails()
  {
    if(localStorage.getItem('email')==null || localStorage.getItem('otpExpSec')==null)
    {
      this.router.navigate(['/']);
      return;
    }
  }

  ngOnInit() {
    this.startTimer();
  }

  ngOnDestroy() {
    if (this.timerInterval) {
      clearInterval(this.timerInterval);
    }
  }

  private startTimer() {
    this.canResend = false;
    var temptimer = this.timer;
    this.timerInterval = setInterval(() => {
      this.timer--;
      if (this.timer <= 0) {
        this.canResend = true;
        clearInterval(this.timerInterval);
      }
    }, 1000);
    this.timer = temptimer;
  }

  onInput(event: any, nextInputId?: string) {
    const input = event.target;
    const value = input.value;

    // Only allow single digits
    if (value.length > 1) {
      input.value = value.slice(0, 1);
    }

    // Auto-focus next input
    if (value && nextInputId) {
      const nextInput = document.getElementById(nextInputId);
      if (nextInput) {
        nextInput.focus();
      }
    }
  }

  getOtpValue(): string {
    const { digit1, digit2, digit3, digit4 } = this.form.value;
    return `${digit1 || ''}${digit2 || ''}${digit3 || ''}${digit4 || ''}`;
  }

  isFormValid(): boolean {
    return this.form.valid && this.getOtpValue().length === 4;
  }

  resend() {
    this.isLoading = true;
    this.loading_message = "Resending OTP ...";
    if (this.canResend)
    {
      const req : VerifyEmailRequest =
      {
        email : this.email,
      }
      this.forgotPasswordService.resendOTP(req).subscribe(
      {
        next:(res=>{
          if(!res || res.success == false)
          {
            this.isLoading = false;
            this.toastService.show("Unable to resend OTP "+res.message);
          }
          this.toastService.show("OTP sent successfully on "+res.email);
          localStorage.setItem('otpExpSec', String(res.otpExpirySec));
          setTimeout(()=>{
            this.isLoading = false;
            this.startTimer();
            this.form.reset();
          }, 3000);
        })
      })
    }
  }

  onSubmit() {
    this.isLoading = true;
    this.loading_message = "Verifying OTP";
    if (this.isFormValid())
    {
      if(this.email!=null)
      {
        const req : OtpVerificationRequest={
          otp : Number(this.getOtpValue()),
          email : this.email?this.email:"",
        }
        this.forgotPasswordService.verifyOtp(req).subscribe({
          next:(res=>{
            if(!res.success||!res)
            {
              this.isLoading = false;
              this.toastService.show("Invalid OTP!");
            }
            else
            {
              this.isLoading = false;
              this.toastService.show("OTP Verified");
              this.router.navigate(['/forgot-password/setup-new-password']);
            }
          })
        })
      }
      else
      {
        this.isLoading = false;
        this.toastService.show("Invalid Email attempt!");
      }
    }
    else {
      this.isLoading = false;
      this.toastService.show("Please enter a valid 4-digit OTP");
    }
  }

  get formattedTimer(): string {
    const minutes = Math.floor(this.timer / 60);
    const seconds = this.timer % 60;
    return `${minutes}:${seconds.toString().padStart(2, '0')}`;
  }
}
