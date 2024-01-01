export interface Project {
    id : number;
    name: string;
    developerId: number;
    deadLine: string | undefined;
    timeSpend: number;
    projectIsCompleted: boolean;
}