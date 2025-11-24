export interface VerifyEmailResponse
{
  success : boolean;
  message : string;
  email : string;
  otpExpirySec : number;
  errors? : string[];
}

export interface VerifyEmailRequest
{
  email:any;
}

export interface ResetPasswordResponse
{
  success:boolean;
  message:string;
  errors?:string[];
}

export interface ResetPasswordRequest
{
  email : any;
  password : string;
  repassword : string;
}

export interface OtpVerificationResponse
{
  success: boolean;
  message : string;
  errors? : string[];
  email : string;
}

export interface OtpVerificationRequest
{
  email : string;
  otpValue : string;
}
