import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { scanReceiptResponse, uploadReceiptResponse } from './dashboard.models';

@Injectable({
  providedIn: 'root',
})
export class DashboardReceiptService {
  private apiURL = 'https://localhost:7253/api/Receipt';

  constructor(private httpClient : HttpClient){}

  uploadReceipt(file: File): Observable<uploadReceiptResponse> {
    const formData = new FormData();
    formData.append("file", file);

    return this.httpClient.post<uploadReceiptResponse>(
      `${this.apiURL}/upload`,
      formData,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`
        }
      }
    );
  }

  scanReceipt(file: File): Observable<scanReceiptResponse> {
    const formData = new FormData();
    formData.append("file", file);

    return this.httpClient.post<scanReceiptResponse>(
      `${this.apiURL}/scan`,
      formData,
      {
        headers: {
          Authorization: `Bearer ${localStorage.getItem("token")}`
        }
      }
    );
  }

}
