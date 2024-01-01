import { DatePipe } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { of, throwError } from 'rxjs';
import { Error } from 'src/Contracts/Error';
import { ProjectRequest } from 'src/Contracts/Requests/ProjectRequest';
import { PaginationProjectResponse, ProjectResponse } from 'src/Contracts/Responses/ProjectResponse';
import { ProjectsComponent } from './projects.component';
import { ProjectService } from './projects.componentService';


describe('ProjectsComponent', () => {
  let component: ProjectsComponent;
  let fixture: ComponentFixture<ProjectsComponent>;
  let projectService: ProjectService;
  let datePipe: DatePipe;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProjectsComponent ],
      imports: [HttpClientTestingModule],
      providers: [DatePipe, ProjectService],
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProjectsComponent);
    component = fixture.componentInstance;
    projectService = TestBed.inject(ProjectService);
    datePipe = TestBed.inject(DatePipe);
  });

  function checkExistButton(idButton: string) {
    const orderByDeadlineButton = fixture.nativeElement.querySelector(idButton);
      expect(orderByDeadlineButton).toBeTruthy();
  }

  function validateProjects() {
    component.projects.forEach(project => {
      const projectNameInput = fixture.nativeElement.querySelector(`#new_name_${project.id}`);
      const projectDeadlineInput = fixture.nativeElement.querySelector(`#new_deadline_${project.id}`);

      expect(projectNameInput).toBeTruthy(); 
      expect(projectDeadlineInput).toBeTruthy(); 
      expect(projectNameInput.ngModel).toBe(project.name);
      expect(projectDeadlineInput.ngModel).toBe(project.deadLine); 
    });
  }

  function addDaysToDate(days: number){
    const date = new Date();

    date.setUTCDate(date.getUTCDate() + days);

    return date;
  }

  function addDaysFormatStringDate(days: number) {
    const date = addDaysToDate(days);

    return stringFormatDate(date);
  }

  function stringFormatDate(date: Date) {
    return datePipe.transform(date, 'yyyy-MM-dd') || '';
  }

  function insertDefaultProject() {
    const error : Error = {
      message : ""
    }

    const projectsResponse: PaginationProjectResponse = {
      isSucess: true,
      error: error,
      projects: [
        {
          id: 1,
          name: 'Project 1',
          developerId: 1,
          projectIsCompleted: false,
          timeSpend: 20,
          deadLine: addDaysFormatStringDate(500),
        },
      ],
      haveMoreProjects: false
    };

    spyOn(projectService, 'GetProjectsByPagination').and.returnValue(of(projectsResponse));

    fixture.detectChanges();
  }

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load first project on initialization', fakeAsync(() => {

    insertDefaultProject();

    expect(component.errorDescription).toBe('');
    expect(component.projects.length).toBe(1);

    fixture.detectChanges();

    fixture.whenStable().then(() => {
      checkExistButton('#order-deadline-button');
      checkExistButton('#new-project-button');
      checkExistButton('#update-project-button');
      checkExistButton('#update-status-button');
      checkExistButton('#update-time-used-button');

      validateProjects();
    });
    tick();
  }));

  it('should create a new project', fakeAsync(() => {

    insertDefaultProject();

    expect(component.errorDescription).toBe('');
    expect(component.projects.length).toBe(1);

    fixture.detectChanges();

    fixture.whenStable().then(() => {
      checkExistButton('#order-deadline-button');
      checkExistButton('#new-project-button');
      checkExistButton('#update-project-button');
      checkExistButton('#update-status-button');
      checkExistButton('#update-time-used-button');

      validateProjects();

      let button = fixture.debugElement.query(By.css('#new-project-button')); // modify here
      button.triggerEventHandler('click', null);
      fixture.detectChanges();
      expect(component.errorDescription).toBe('Invalid name.');

      // After receiving a error input right values to insert new project
      const newNameInput: HTMLInputElement = fixture.nativeElement.querySelector('#new_name');
      const newDeadlineInput: HTMLInputElement = fixture.nativeElement.querySelector('#new_deadline');

      component.newProject = {
        name: 'My new project',
        deadLine: addDaysFormatStringDate(600),
      };

      newNameInput.value = component.newProject.name;
      newNameInput.dispatchEvent(new Event('input'));

      newDeadlineInput.value = component.newProject.deadLine;
      newDeadlineInput.dispatchEvent(new Event('input')); 

      const clickButtonCreateProject = fixture.nativeElement.querySelector('#new-project-button');
      clickButtonCreateProject.click();

      fixture.detectChanges();
      tick();

      const projectsNewResponse: ProjectResponse = {
        isSucess: true,
        error: {
          message: ''
        },
        projects: [
          {
            id: 2,
            name: 'My new project',
            developerId: 1,
            projectIsCompleted: false,
            timeSpend: 0,
            deadLine: addDaysFormatStringDate(600),
          },
        ],
      };
      spyOn(projectService, 'InsertNewProject').and.callFake((newProject: ProjectRequest) => {
          return of(projectsNewResponse);
      });
      component.insertProjects(component.newProject)

      fixture.detectChanges();
      tick();

      fixture.whenStable().then(() => {
        expect(component.projects.length).toBe(2);
        
        validateProjects();
      });

    });
    tick();
  }));

  it('only invalid requests', fakeAsync(() => {

    insertDefaultProject();

    expect(component.errorDescription).toBe('');
    expect(component.projects.length).toBe(1);

    fixture.detectChanges();

    fixture.whenStable().then(() => {
      checkExistButton('#order-deadline-button');
      checkExistButton('#new-project-button');
      checkExistButton('#update-project-button');
      checkExistButton('#update-status-button');
      checkExistButton('#update-time-used-button');

      validateProjects();

      let button = fixture.debugElement.query(By.css('#new-project-button')); // modify here
      button.triggerEventHandler('click', null);
      fixture.detectChanges();
      expect(component.errorDescription).toBe('Invalid name.');

      // After receiving a error input right values to insert new project
      const newNameInput: HTMLInputElement = fixture.nativeElement.querySelector('#new_name');
      const newDeadlineInput: HTMLInputElement = fixture.nativeElement.querySelector('#new_deadline');

      component.newProject = {
        name: 'My new project',
        deadLine: '2024-12-229',
      };

      newNameInput.value = component.newProject.name;
      newNameInput.dispatchEvent(new Event('input'));

      newDeadlineInput.value = component.newProject.deadLine;
      newDeadlineInput.dispatchEvent(new Event('input')); 

      let clickButtonCreateProject = fixture.nativeElement.querySelector('#new-project-button');
      clickButtonCreateProject.click();
      expect(component.errorDescription).toBe('Invalid date Deadline.');
      
      component.newProject = {
        name: 'My new project',
        deadLine:  addDaysFormatStringDate(-200),
      };

      newNameInput.value = component.newProject.name;
      newNameInput.dispatchEvent(new Event('input'));

      newDeadlineInput.value = component.newProject.deadLine;
      newDeadlineInput.dispatchEvent(new Event('input')); 

      clickButtonCreateProject = fixture.nativeElement.querySelector('#new-project-button');
      clickButtonCreateProject.click();
      expect(component.errorDescription).toBe('The date is on the past.');

      component.newProject = {
        name: 'My new project',
        deadLine: '2024-02-30',
      };

      newNameInput.value = component.newProject.name;
      newNameInput.dispatchEvent(new Event('input'));

      newDeadlineInput.value = component.newProject.deadLine;
      newDeadlineInput.dispatchEvent(new Event('input')); 

      clickButtonCreateProject = fixture.nativeElement.querySelector('#new-project-button');
      clickButtonCreateProject.click();
      expect(component.errorDescription).toBe('Invalid date Deadline.');

      component.newProject = {
        name: 'My new project',
        deadLine: '2022/12/29',
      };

      newNameInput.value = component.newProject.name;
      newNameInput.dispatchEvent(new Event('input'));

      newDeadlineInput.value = component.newProject.deadLine;
      newDeadlineInput.dispatchEvent(new Event('input')); 

      clickButtonCreateProject = fixture.nativeElement.querySelector('#new-project-button');
      clickButtonCreateProject.click();
      expect(component.errorDescription).toBe('Invalid date Deadline.');
    });
    tick();
  }));

  it('should handle error on getCallProjects', fakeAsync(() => {
    const errorResponse = new HttpErrorResponse({ status: 404, statusText: 'Not Found' });

    spyOn(projectService, 'GetProjectsByPagination').and.returnValue(throwError(errorResponse));

    fixture.detectChanges();

    expect(component.errorDescription).toBe('Http failure response for (unknown url): 404 Not Found');
    
  }));

});

