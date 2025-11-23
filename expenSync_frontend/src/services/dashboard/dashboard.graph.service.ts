import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { GraphDataResponse } from './dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class DashboardGraphService {

  private apiUrl = 'https://localhost:7253/api/transaction';

  constructor(private httpClient : HttpClient){}

  getGraphData():Observable<GraphDataResponse>
  {
    return this.httpClient.get<GraphDataResponse>(`${this.apiUrl}/GetGraphData`);
  }

}
