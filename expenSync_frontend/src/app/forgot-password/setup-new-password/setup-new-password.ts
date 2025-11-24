
import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Output } from '@angular/core';
import { FormControl, FormGroup, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { Router, RouterModule } from '@angular/router';
import { ForgotPasswordService } from '../../../services/forgot-password/forgot-password.service';
import { ResetPasswordRequest } from '../../../services/forgot-password/forgot-password.modals';
import { ForgotPasswordToastService } from '../../../services/forgot-password/forgot-password.toast.service';

@Component({
  selector: 'app-setup-new-password',
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './setup-new-password.html',
  styleUrl: './setup-new-password.css',
})
export class SetupNewPassword {

  isLoading : boolean = false;
  loadingMessage : string = '';

  constructor(private router : Router, private forgotPasswordService : ForgotPasswordService, private toastService : ForgotPasswordToastService){}
  password = '';
  confirmPassword = '';

  submitted = false;

  passwordValidations = {
    length: false,
    upper: false,
    lower: false,
    number: false,
    special: false
  };

  validatePassword() {
    const pwd = this.password || '';
    this.passwordValidations.length = pwd.length >= 6;
    this.passwordValidations.upper = /[A-Z]/.test(pwd);
    this.passwordValidations.lower = /[a-z]/.test(pwd);
    this.passwordValidations.number = /[0-9]/.test(pwd);
    this.passwordValidations.special = /[!@#$%^&*(),.?":{}|<>]/.test(pwd);
  }

  allPasswordValidationsMet() {
    return Object.values(this.passwordValidations).every(Boolean);
  }

  get passwordsMatch() {
    return this.password && this.confirmPassword && this.password === this.confirmPassword;
  }

  onSubmit() {
    this.submitted = true;
    this.isLoading = true;
    this.loadingMessage = "Setting up your new Password";

    if (
      this.allPasswordValidationsMet() &&
      this.passwordsMatch
    ) {
      const req : ResetPasswordRequest = {
        email : localStorage.getItem('email'),
        password : this.password,
        confirmPassword : this.confirmPassword
      }
      this.forgotPasswordService.resetPassword(req).subscribe({
        next:(res=>{
          if(!res || !res.success)
          {
            this.isLoading = false;
            this.toastService.show("Couldn't reset password" + res.message);
          }
          else
          {
            this.resetForm();
            this.isLoading = false;
            this.toastService.show("Password updated successfully!");
            setTimeout(()=>{
              this.router.navigate(['/landingpage/signin']);
            },3000);
          }
        })
      })
    }
  }

  private resetForm() {
    this.password = '';
    this.confirmPassword = '';
    this.submitted = false;
    this.passwordValidations = {
      length: false,
      upper: false,
      lower: false,
      number: false,
      special: false
    };
  }
}
