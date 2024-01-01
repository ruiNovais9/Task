import { HttpClient, HttpHeaders, HttpParams } from "@angular/common/http";
import { Injectable } from "@angular/core";


const httpOptions = {
    headers: new HttpHeaders({
      'Content-Type':  'application/json',
      Authorization: 'my-auth-token'
    })
};

@Injectable ({
    providedIn: 'root',
})

export class HttpCalls {
    constructor(private http: HttpClient) {

    }

    public DoPutWithRequest<T>(url: string, request: any) {
        return this.http.put<T>(url, request, httpOptions).pipe();
    }

    public DoPut<T>(url: string) {
        return this.http.put<T>(url, httpOptions).pipe();
    }

    public DoPutWithParameters<T>(parameters: { key: string; value: any }[], url: string) {
        let queryParams = new HttpParams();
        for (const param of parameters) {
            queryParams = queryParams.append(param.key, param.value.toString());
        }
        return this.http.put<T>(url,  {params: queryParams}, httpOptions).pipe();
    }

    public DoPost<T>(request: any, url: string) {
        return this.http.post<T>(url, request, httpOptions).pipe();
    }

    public DoGet<T>(url: string) {
        return this.http.get<T>(url, httpOptions).pipe();
    }

    public DoGetWithParameter<T>(url: string) {
        return this.http.get<T>(url).pipe();
    }
}