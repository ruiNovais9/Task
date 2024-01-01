import { environment } from "src/environments/environment";

export const enum Endpoints {
    GetAllProjects,
    GetProjectsByPagination,
    OrderByDeadLineProjects,
    InsertNewProject,
    UpdateProject,
    UpdateStatusProject,
    UpdateTimeSpendProject,
}

// Later in your code:
export const apiEndpoints = {
    [Endpoints.GetAllProjects]: environment.API_URL,
    [Endpoints.GetProjectsByPagination]: environment.API_URL,
    [Endpoints.OrderByDeadLineProjects]: environment.API_URL,
    [Endpoints.InsertNewProject]: environment.API_URL,
    [Endpoints.UpdateProject]: environment.API_URL,
    [Endpoints.UpdateStatusProject]: environment.API_URL + "{id}/status/{status}",
    [Endpoints.UpdateTimeSpendProject]: environment.API_URL + "{id}/timeSpend/{timeUsed}",
  };
