import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Customer } from './customer';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CustomerService {

  constructor(private http: HttpClient) { }

  search(term: string): Observable<Customer[]> {
    return this.http.get<Customer[]>(`/api/customers/search`, {
      params: { term }
    });
  }
}
