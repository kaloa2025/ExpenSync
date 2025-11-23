import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AddNewCategoryRequest, AddNewCategoryResponse, DeleteCategoryResponse, FetchCategoriesResponse, FetchCategoryResponse, FetchIconsResponse, UpdateCategoryRequest, UpdateCategoryResponse } from './dashboard.models';

@Injectable({
  providedIn: 'root'
})
export class DashboardCategory {
  private apiUrl = 'https://localhost:7253/api/category';

  constructor(private http:HttpClient){}

  getAllIcons():Observable<FetchIconsResponse>{
    return this.http.get<FetchIconsResponse>(`${this.apiUrl}/getAllIcons`);
  }

  getAllCategories():Observable<FetchCategoriesResponse>{
    return this.http.get<FetchCategoriesResponse>(`${this.apiUrl}/getAllCategories`);
  }

  getCategory(categoryId: number): Observable<FetchCategoryResponse> {
    return this.http.get<FetchCategoryResponse>(`${this.apiUrl}/getCategory/${categoryId}`);
  }

  deleteCategory(categoryId:number):Observable<DeleteCategoryResponse>{
    return this.http.delete<DeleteCategoryResponse>(`${this.apiUrl}/deleteCategory/${categoryId}`,);
  }

  addCategory(req: AddNewCategoryRequest): Observable<AddNewCategoryResponse> {
    return this.http.post<AddNewCategoryResponse>(`${this.apiUrl}/AddNewCategory`, req);
  }

  updateCategory(categoryId: number, req: UpdateCategoryRequest): Observable<UpdateCategoryResponse> {
    return this.http.put<UpdateCategoryResponse>(
      `${this.apiUrl}/updateCategory/${categoryId}`, req
    );
  }

}
