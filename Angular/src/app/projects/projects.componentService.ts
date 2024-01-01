import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { ProjectRequest } from "src/Contracts/Requests/ProjectRequest";
import { ProjectUpdate } from "src/Contracts/Requests/ProjectUpdate";
import { PaginationProjectResponse, ProjectResponse } from "src/Contracts/Responses/ProjectResponse";
import { Endpoints, apiEndpoints } from "../endPoints";
import { HttpCalls } from "../httpCalls";

@Injectable ({
    providedIn: 'root',
})

export class ProjectService {
    
    constructor(private httpCalls: HttpCalls) { }

    public GetProjects() : Observable<PaginationProjectResponse> {
        return this.httpCalls.DoGet<PaginationProjectResponse>(apiEndpoints[Endpoints.GetAllProjects]);
    }

    public GetProjectsByPagination(pageIndex: number, numberProjects: number) : Observable<PaginationProjectResponse> {
        const endpoint = apiEndpoints[Endpoints.GetProjectsByPagination] + pageIndex + '/' + numberProjects;
        return this.httpCalls.DoGet<PaginationProjectResponse>(endpoint);
    }

    public InsertNewProject(request: ProjectRequest) : Observable<ProjectResponse>{
        return this.httpCalls.DoPost<ProjectResponse>(request, apiEndpoints[Endpoints.InsertNewProject]);
    }

    public UpdateStatusOfProject(id: number, status: boolean) : Observable<ProjectResponse>{
        const endpoint = apiEndpoints[Endpoints.UpdateStatusProject]
                        .replace('{id}', id.toString())
                        .replace('{status}', status.toString());

        return this.httpCalls.DoPut<ProjectResponse>(endpoint);
    }

    public UpdateTimeUsedOnProject(id: number, timeUsed: number) : Observable<ProjectResponse>{
        const endpoint = apiEndpoints[Endpoints.UpdateTimeSpendProject]
                        .replace('{id}', id.toString())
                        .replace('{timeUsed}', timeUsed.toString());

        return this.httpCalls.DoPut<ProjectResponse>(endpoint);
    }

    public OrderByDeadLine(orderBy: boolean, pageIndex: number, numberProjects: number) : Observable<PaginationProjectResponse>{
        const endpoint = apiEndpoints[Endpoints.OrderByDeadLineProjects] + orderBy + '/' + pageIndex + '/' + numberProjects;
        return this.httpCalls.DoGetWithParameter<PaginationProjectResponse>(endpoint);
    }

    public UpdateNewProject(request: ProjectUpdate) : Observable<ProjectResponse>{
        return this.httpCalls.DoPutWithRequest<ProjectResponse>(apiEndpoints[Endpoints.UpdateProject], request);
    }
}