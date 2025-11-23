import { Injectable } from '@angular/core';
import { EditProfileRequest } from '../auth/auth.models';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EditProfileResponse } from './dashboard.models';

@Injectable({
  providedIn: 'root'
})

export class DashboardProfileService {

  private apiUrl = 'https://localhost:7253/api/profile';
  constructor(private httpClient : HttpClient){}

  updateProfile(req :EditProfileRequest):Observable<EditProfileResponse>
  {
    return this.httpClient.put<EditProfileResponse>(`${this.apiUrl}/update`,req);
  }

}
