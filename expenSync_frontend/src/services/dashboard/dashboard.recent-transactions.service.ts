import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GetTransactionsByDateResponse } from './dashboard.models';

@Injectable({
  providedIn: 'root',
})
export class DashboardRecentTransactionsService {

  private apiUrl = 'https://localhost:7253/api/transaction';

  constructor(private httpClient : HttpClient){}

  getRecentTransactions():Observable<GetTransactionsByDateResponse>{
    return this.httpClient.get<GetTransactionsByDateResponse>(`${this.apiUrl}/GetRecentTransactions`);
  }
}


