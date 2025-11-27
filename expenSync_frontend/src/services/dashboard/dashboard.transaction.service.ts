import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { CategoryList, CreateTransactionRequest, ExpenseTypeList, ModeOfPaymentList, Transaction, TransactionResponse } from './dashboard.models';

@Injectable({
  providedIn: 'root',
})
export class DashboardTransactionService {

  private apiUrl = 'https://localhost:7253/api/transaction';

  constructor(private httpClient : HttpClient){}

  getAllCategories():Observable<CategoryList>{
    return this.httpClient.get<CategoryList>(`${this.apiUrl}/GetAllCategories`);
  }

  getAllModeOfPayment():Observable<ModeOfPaymentList>{
    return this.httpClient.get<ModeOfPaymentList>(`${this.apiUrl}/GetAllModeOfPayments`);
  }

  getAllExpenseType():Observable<ExpenseTypeList>{
    return this.httpClient.get<ExpenseTypeList>(`${this.apiUrl}/GetAllExpenseTypes`);
  }

  createTransaction(req:CreateTransactionRequest):Observable<TransactionResponse> {
    return this.httpClient.post<TransactionResponse>(`${this.apiUrl}/AddNewExpense`,req);
  }

  deleteTransaction(transactionId:number):Observable<TransactionResponse>{
    return this.httpClient.delete<TransactionResponse>(`${this.apiUrl}/DeleteTransaction/${transactionId}`);
  }

  editTransaction(req:CreateTransactionRequest, transactionId?:Number):Observable<TransactionResponse>
  {
    return this.httpClient.put<TransactionResponse>(`${this.apiUrl}/EditExpense/${transactionId}`, req);
  }

  getReport():Observable<TransactionResponse>
  {
    return this.httpClient.get<TransactionResponse>(`${this.apiUrl}/GetReport`);
  }

}
