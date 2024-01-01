import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Projects } from 'src/Contracts/Projects';
import { ProjectRequest } from 'src/Contracts/Requests/ProjectRequest';
import { ProjectUpdate } from 'src/Contracts/Requests/ProjectUpdate';
import { PaginationProjectResponse, ProjectResponse } from 'src/Contracts/Responses/ProjectResponse';
import { ProjectService } from './projects.componentService';

@Component({
  selector: 'app-projects',
  templateUrl: './projects.component.html',
  styleUrls: ['./projects.component.scss']
})
export class ProjectsComponent implements OnInit {
  public projects: Projects[] = [];
  public newProject: ProjectRequest = { name: "", deadLine: ''  };
  public errorDescription: string = "";
  private numberProjectsPagination = 25;
  constructor(private service: ProjectService, private datePipe: DatePipe) { }

  ngOnInit(): void {
    this.getCallProjects();
  }
  // Missing adding pagination on html .
  getCallProjects() {
    this.errorDescription = '';
    let pageIndex = 0;
    this.service.GetProjectsByPagination(pageIndex, this.numberProjectsPagination).subscribe({
      next: (x:PaginationProjectResponse) => {
        if (!x.isSucess && x.error && x.error.message){
          this.errorDescription = x.error.message;
        }
        else {
          x.projects.forEach(element => {
            this.projects.push(
              { 
                id: element.id, 
                name: element.name,                
                developerId: element.developerId, 
                projectIsCompleted: element.projectIsCompleted,
                timeSpend: element.timeSpend,
                deadLine: element.deadLine ? this.datePipe.transform(element.deadLine, 'yyyy-MM-dd') || '' : undefined,
                timeOnProject: 0
              });
          });

          if(x.haveMoreProjects) {
            this.getProjectsByPagination(pageIndex+1, this.numberProjectsPagination);
          }
        }
      },
      error: (error: HttpErrorResponse) => {
        this.handleErrors(error);
      }

    });
  }

  getProjectsByPagination(pageIndex = 0, numberProjects = this.numberProjectsPagination) : void {
    this.service.GetProjectsByPagination(pageIndex, numberProjects).subscribe({
        next: (x:PaginationProjectResponse) => {
          if (!x.isSucess && x.error && x.error.message){
            this.errorDescription = x.error.message;
          }
          else {
            this.errorDescription = "";
            
            x.projects.forEach(element => {
              this.projects.push(
                { 
                  id: element.id, 
                  name: element.name,                
                  developerId: element.developerId, 
                  projectIsCompleted: element.projectIsCompleted,
                  timeSpend: element.timeSpend,
                  deadLine: element.deadLine ? this.datePipe.transform(element.deadLine, 'yyyy-MM-dd') || '' : undefined,
                  timeOnProject: 0
                });
            });
            
            if(x.haveMoreProjects) {
              this.getProjectsByPagination(pageIndex+1, numberProjects);
            }
          }
        },
        error: (error: HttpErrorResponse) => {
          this.handleErrors(error);
        }
    })
  }

  updateProject(project: ProjectUpdate) : void {
    this.errorDescription = '';
    if (this.validatedRequest(project)) {
      this.service.UpdateNewProject(project).subscribe({
        next: (x:ProjectResponse) => {
          if (!x.isSucess && x.error && x.error.message){
            this.errorDescription = x.error.message;
          }
          else {
            this.errorDescription = "";
          }
        },
        error: (error: HttpErrorResponse) => {
          this.handleErrors(error);
        }
      })
    }
    
  }

  updateStatusProject(id: number, status: boolean) : void {
    this.errorDescription = '';
    this.service.UpdateStatusOfProject(id, status).subscribe({
      next: (x:ProjectResponse) => {
        if (!x.isSucess && x.error && x.error.message){
          this.errorDescription = x.error.message;
        }
        else {
          this.errorDescription = "";

          const projectFinded = this.projects.find(x => x.id == id);
          if (projectFinded){
            x.projects.forEach(element => {
              projectFinded.deadLine = element.deadLine ? this.datePipe.transform(element.deadLine, 'yyyy-MM-dd') || '' : undefined
              projectFinded.projectIsCompleted = element.projectIsCompleted;
              projectFinded.name = element.name
            })
          }
        }
      },
      error: (error: HttpErrorResponse) => {
        this.handleErrors(error);
      }
    })
  }

  validatedRequest(newProject: ProjectRequest | ProjectUpdate) : boolean {
    if (!newProject) {
      this.errorDescription = "Invalid project.";
      return false;
    } else if (!newProject.name) {
      this.errorDescription = "Invalid name.";
      return false;
    } else if (!newProject.deadLine) {
      this.errorDescription = "Invalid date Deadline.";
      return false;
    } else {
      const dateFormat = /^\d{4}-\d{2}-\d{2}$/;
      const isValidFormat = dateFormat.test(newProject.deadLine.toString());
      var todaysDate = new Date();

      if (!isValidFormat) {
        this.errorDescription = "Invalid date Deadline.";
        return false;
      } else if (isValidFormat && todaysDate > new Date(newProject.deadLine)){
        this.errorDescription = "The date is on the past.";
        return false;
      } 

      const [year, month, day] = newProject.deadLine.split('-').map(Number);
      const lastDayOfMonth = new Date(year, month, 0).getDate();

      if (day > lastDayOfMonth) {
        this.errorDescription = "Invalid date Deadline.";
        return false;
      }

      const deadlineDate = new Date(newProject.deadLine);

      if (todaysDate > deadlineDate) {
        this.errorDescription = "Invalid date Deadline.";
        return false;
      }

      let deadLineDate = new Date(newProject.deadLine);
      if (isNaN(deadLineDate.getTime())){
        this.errorDescription = "Invalid date Deadline.";
        return false;
      }
    }
    return true;
  }

  insertProjects(newProject: ProjectRequest) : void {
    this.errorDescription = '';
    if (this.validatedRequest(newProject)) {
      this.service.InsertNewProject(newProject).subscribe({
        next: (x:ProjectResponse) => {
          if (!x.isSucess || (x.error && x.error.message)){
            this.errorDescription = x.error.message;
          }
          else {
            this.errorDescription = "";
            this.newProject = {
              name: "",
              deadLine: ''
            }
            
            x.projects.forEach(element => {
              this.projects.push({
                id: element.id,
                deadLine: element.deadLine ? this.datePipe.transform(element.deadLine, 'yyyy-MM-dd') || '' : undefined,
                developerId: element.developerId,
                name: element.name,
                projectIsCompleted: element.projectIsCompleted,
                timeSpend: element.timeSpend,
                timeOnProject: 0
              });
            })
          }
        },
        error: (error: HttpErrorResponse) => {
          this.handleErrors(error);
        }
      })
    }
  }

  orderByDeadLine(pageIndex = 0, numberProjects = this.numberProjectsPagination) : void {
    // Always true because i want always order by asc
    if (pageIndex == 0) {
      this.projects = [];
    }
    
    this.service.OrderByDeadLine(true, pageIndex, numberProjects).subscribe({
        next: (x:PaginationProjectResponse) => {
          if (!x.isSucess && x.error && x.error.message){
            this.errorDescription = x.error.message;
          }
          else {
            this.errorDescription = "";
            
            x.projects.forEach(element => {
              this.projects.push(
                { 
                  id: element.id, 
                  name: element.name,                
                  developerId: element.developerId, 
                  projectIsCompleted: element.projectIsCompleted,
                  timeSpend: element.timeSpend,
                  deadLine: element.deadLine ? this.datePipe.transform(element.deadLine, 'yyyy-MM-dd') || '' : undefined,
                  timeOnProject: 0
                });
            });
            
            if(x.haveMoreProjects) {
              this.orderByDeadLine(pageIndex+1, numberProjects);
            }
          }
        },
        error: (error: HttpErrorResponse) => {
          this.handleErrors(error);
        }
    })
  }

  addTimeUsedOnProject(id: number, timeUsed: number) : void {
    this.service.UpdateTimeUsedOnProject(id, timeUsed).subscribe({
      next: (x:ProjectResponse) => {
        if (!x.isSucess && x.error && x.error.message){
          this.errorDescription = x.error.message;
        }
        else {
          this.errorDescription = "";
          const projectFinded = this.projects.find(x => x.id == id);
          if (projectFinded){
            projectFinded.timeOnProject = 0;
            x.projects.forEach(element => {
              projectFinded.deadLine = element.deadLine ? this.datePipe.transform(element.deadLine, 'yyyy-MM-dd') || '' : undefined
              projectFinded.projectIsCompleted = element.projectIsCompleted;
              projectFinded.name = element.name
              projectFinded.timeSpend = element.timeSpend;
            })
          }
        }
      },
      error: (error: HttpErrorResponse) => {
        this.handleErrors(error);
      }
    })
  }

  private handleErrors(error: HttpErrorResponse){
    let errorProjectResponse = this.handleError(error) as ProjectResponse;

    if (errorProjectResponse && errorProjectResponse.error) {
      this.errorDescription = errorProjectResponse.error.message;
    } else {
      this.errorDescription = error.message;
    }
    
  }

  private handleError(error: HttpErrorResponse) : ProjectResponse {
    return error.error as ProjectResponse;
  }
}
