import { Component } from '@angular/core';
import { RouterModule } from "@angular/router";
import { EnterEmail } from "./enter-email/enter-email";
import { OtpVerification } from "./otp-verification/otp-verification";
import { SetupNewPassword } from "./setup-new-password/setup-new-password";
import { CommonModule } from '@angular/common';
import { ForgotPasswordToastService } from '../../services/forgot-password/forgot-password.toast.service';

@Component({
  selector: 'app-forgot-password',
  imports: [RouterModule, CommonModule],
  templateUrl: './forgot-password.html',
  styleUrl: './forgot-password.css',
})
export class ForgotPassword {
  toastMessage: string = '';

  constructor(private toastService : ForgotPasswordToastService){
    this.toastService.toast$.subscribe(msg => {
      this.toastMessage = msg;
      setTimeout(() => this.toastMessage = '', 2500);
    });
  }


}
