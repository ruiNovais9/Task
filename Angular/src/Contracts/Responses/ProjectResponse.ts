import { Error } from "../Error";
import { Project } from "../Project";

export interface ProjectResponse {
    projects: Project[];
    isSucess: boolean;
    error: Error;
}

export interface PaginationProjectResponse extends ProjectResponse{
    haveMoreProjects: boolean;
}

